using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ProfileService.API.Filters;
using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Dtos.Responses;
using ProfileService.Business.Services.Interfaces;
using Shared.Contracts.Responses;

namespace ProfileService.API.Controllers
{
    /// <summary>
    /// Endpoint nội bộ cho service khác gọi (service-to-service).
    /// Không dùng JWT vì request không gắn với user cụ thể nào,
    /// dùng khoá dùng chung qua <see cref="InternalApiKeyFilter"/>.
    ///
    /// Route tách riêng khỏi /api/Profile (public, có [Authorize]) để không
    /// nhầm lẫn và dễ siết quyền riêng cho từng nhóm.
    /// </summary>
    [ApiController]
    [Route("api/internal/profiles")]
    [ServiceFilter(typeof(InternalApiKeyFilter))]
    [Produces("application/json")]
    public class InternalProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public InternalProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        /// <summary>
        /// Tạo profile rỗng cho user vừa đăng ký: chỉ có UserId, FullName, WorkEmail,
        /// các trường còn lại để trống cho user/admin bổ sung sau.
        /// Idempotent: nếu profile đã tồn tại thì trả về chính profile đó (200)
        /// thay vì lỗi 409, để caller có thể gọi lại an toàn.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProfileDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse<ProfileDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<ApiResponse<ProfileDto>>> CreateFromRegistration(
            [FromBody] InternalCreateProfileRequest request)
        {
            var existing = await _profileService.GetProfileByUserIdAsync(request.UserId);
            if (existing != null)
            {
                return Ok(ApiResponse<ProfileDto>.SuccessResult(
                    existing, "Profile đã tồn tại, trả về bản ghi hiện có."));
            }

            var created = await _profileService.CreateProfileAsync(new ProfileRequest
            {
                UserId = request.UserId,
                FullName = request.FullName,
                WorkEmail = request.WorkEmail,
                // Các trường còn lại giữ nguyên giá trị mặc định (chuỗi rỗng / null)
                // để user bổ sung sau qua màn hình cập nhật hồ sơ.
            });

            return Created(
                $"/api/internal/profiles/{created.Id}",
                ApiResponse<ProfileDto>.SuccessResult(created, "Đã tạo hồ sơ cho người dùng mới."));
        }

        /// <summary>
        /// Hoàn tác hồ sơ vừa tạo khi IdentityService không lưu được user.
        /// Trả 204 kể cả khi không có hồ sơ (idempotent) để caller có thể
        /// gọi lại an toàn trong cơ chế compensating transaction.
        /// </summary>
        [HttpDelete("by-user/{userId:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> DeleteByUserId(Guid userId)
        {
            await _profileService.DeleteProfileByUserIdAsync(userId);
            return NoContent();
        }
    }
}

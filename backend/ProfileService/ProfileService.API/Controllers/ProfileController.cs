using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Dtos.Responses;
using ProfileService.Business.Services.Interfaces;
using Shared.Contracts.Exceptions;
using Shared.Contracts.Responses;

namespace ProfileService.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    [Produces("application/json")]
    public class ProfileController : ControllerBase
    {
        private readonly IProfileService _profileService;

        public ProfileController(IProfileService profileService)
        {
            _profileService = profileService;
        }

        [HttpGet]
        [ProducesResponseType(typeof(ApiResponse<PagedResult<ProfileDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<PagedResult<ProfileDto>>>> GetAll(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20)
        {
            var result = await _profileService.GetProfilesAsync(page, pageSize);
            return Ok(ApiResponse<PagedResult<ProfileDto>>.SuccessResult(result));
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProfileDto>>> GetById(Guid id)
        {
            var result = await _profileService.GetProfileByIdAsync(id);
            if (result == null) throw new NotFoundException("Không tìm thấy hồ sơ người dùng.");
            return Ok(ApiResponse<ProfileDto>.SuccessResult(result));
        }

        [HttpGet("by-user/{userId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<ApiResponse<ProfileDto>>> GetByUserId(Guid userId)
        {
            // Không còn 404: service tự tạo hồ sơ rỗng nếu chưa có, vì "chưa có
            // hồ sơ" là trạng thái bình thường với user không đi qua luồng đăng ký
            // chứ không phải lỗi.
            var result = await _profileService.GetProfileByUserIdAsync(userId);
            return Ok(ApiResponse<ProfileDto>.SuccessResult(result));
        }

        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<ProfileDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status409Conflict)]
        public async Task<ActionResult<ApiResponse<ProfileDto>>> Create([FromBody] ProfileRequest request)
        {
            var result = await _profileService.CreateProfileAsync(request);
            return CreatedAtAction(nameof(GetById), new { id = result.Id }, ApiResponse<ProfileDto>.SuccessResult(result));
        }

        [HttpPut("by-user/{userId:guid}")]
        [ProducesResponseType(typeof(ApiResponse<ProfileDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<ApiResponse<ProfileDto>>> Update(Guid userId, [FromBody] ProfileRequest request)
        {
            var result = await _profileService.UpdateProfileAsync(userId, request);
            return Ok(ApiResponse<ProfileDto>.SuccessResult(result));
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _profileService.DeleteProfileAsync(id);
            return NoContent();
        }
    }
}

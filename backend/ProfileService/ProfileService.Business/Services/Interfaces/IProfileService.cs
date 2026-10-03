using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Dtos.Responses;
using Shared.Contracts.Responses;

namespace ProfileService.Business.Services.Interfaces
{
    public interface IProfileService
    {
        Task<PagedResult<ProfileDto>> GetProfilesAsync(int page, int pageSize);
        Task<ProfileDto?> GetProfileByIdAsync(Guid id);

        /// <summary>
        /// Lấy hồ sơ theo UserId, tự tạo hồ sơ rỗng nếu chưa có nên không bao giờ
        /// trả null cho user hợp lệ.
        /// </summary>
        Task<ProfileDto> GetProfileByUserIdAsync(Guid userId);

        Task<ProfileDto> CreateProfileAsync(ProfileRequest request);
        Task<ProfileDto> UpdateProfileAsync(Guid userId, ProfileRequest request);
        Task DeleteProfileAsync(Guid id);
        Task<bool> DeleteProfileByUserIdAsync(Guid userId);
    }
}

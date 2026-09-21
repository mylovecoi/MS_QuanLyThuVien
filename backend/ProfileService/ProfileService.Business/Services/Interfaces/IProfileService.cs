using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Dtos.Responses;

namespace ProfileService.Business.Services.Interfaces
{
    public interface IProfileService
    {
        Task<IEnumerable<ProfileDto>> GetAllProfilesAsync();
        Task<ProfileDto?> GetProfileByIdAsync(Guid id);
        Task<ProfileDto?> GetProfileByUserIdAsync(Guid userId);
        Task<ProfileDto> CreateProfileAsync(ProfileRequest request);
        Task<ProfileDto> UpdateProfileAsync(Guid userId, ProfileRequest request);
        Task<bool> DeleteProfileAsync(Guid id);
    }
}

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ProfileService.Business.Dtos;

namespace ProfileService.Business.Services.Interfaces
{
    public interface IUserProfileService
    {
        Task<IEnumerable<UserProfileDto>> GetAllProfilesAsync();
        Task<UserProfileDto?> GetProfileByIdAsync(Guid id);
        Task<UserProfileDto?> GetProfileByUserIdAsync(Guid userId);
        Task<UserProfileDto> CreateProfileAsync(CreateUserProfileRequest request);
        Task<UserProfileDto> UpdateProfileAsync(Guid userId, UpdateUserProfileRequest request);
        Task<bool> DeleteProfileAsync(Guid id);
    }
}

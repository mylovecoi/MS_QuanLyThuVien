using System;
using System.Threading.Tasks;
using IdentityService.Business.Dtos.Requests;
using IdentityService.Business.Dtos.Responses;
using Shared.Contracts.Responses;

namespace IdentityService.Business.Services.Interfaces
{
    public interface IUserService
    {
        Task<PagedResult<UserDetailResponse>> GetUsersAsync(int page, int pageSize, string? search);
        Task<UserDetailResponse> GetUserByIdAsync(Guid id);
        Task<UserDetailResponse> CreateUserAsync(CreateUserRequest request);
        Task<UserDetailResponse> UpdateUserAsync(Guid id, UpdateUserRequest request);
        Task<UserDetailResponse> UpdateStatusAsync(Guid id, ChangeUserStatusRequest request);
        Task ResetPasswordAsync(Guid id, ResetPasswordRequest request);
        Task DeleteUserAsync(Guid id);
    }
}

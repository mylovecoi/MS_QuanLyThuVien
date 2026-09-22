using IdentityService.Business.Dtos.Requests;
using IdentityService.Business.Dtos.Responses;

namespace IdentityService.Business.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserResponse> RegisterAsync(RegisterRequest request);
        Task<UserResponse> GetProfileAsync(Guid userId);
        Task LogoutAsync(string accessToken, string? refreshToken);
        Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request);
    }
}

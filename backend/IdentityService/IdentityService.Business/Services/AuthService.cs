using IdentityService.Business.Constants;
using IdentityService.Business.Dtos.Requests;
using IdentityService.Business.Dtos.Responses;
using IdentityService.Business.Exceptions;
using IdentityService.Business.Helpers;
using IdentityService.Business.Mappers;
using IdentityService.DataAccess.Entities;
using IdentityService.DataAccess.Repositories.Interfaces;
using Microsoft.Extensions.Configuration;

namespace IdentityService.Business.Services
{
    public class AuthService(
        IUserRepository userRepository,
        IRefreshTokenRepository refreshTokenRepository,
        IInvalidatedTokenRepository invalidatedTokenRepository,
        IRoleRepository roleRepository,
        IConfiguration configuration) : IAuthService
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
        private readonly IInvalidatedTokenRepository _invalidatedTokenRepository = invalidatedTokenRepository;
        private readonly IRoleRepository _roleRepository = roleRepository;
        private readonly IConfiguration _configuration = configuration;

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetWithRolesAndPermissionsByUsernameAsync(request.Username);
            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new BadRequestException(ApplicationConstants.ErrorMessages.InvalidCredentials);
            }

            return await GenerateAuthResponseAsync(user);
        }

        public async Task<UserResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                throw new BadRequestException(ApplicationConstants.ErrorMessages.UsernameAlreadyExists);
            }

            var defaultRole = await _roleRepository.GetByCodeAsync("USER");

            var newUser = new UserEntity
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                FullName = request.FullName,
                Role = ApplicationConstants.Roles.User
            };

            if (defaultRole != null)
            {
                newUser.UserRoles.Add(new UserRoleEntity
                {
                    UserId = newUser.Id,
                    RoleId = defaultRole.Id
                });
            }

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            var userWithRoles = await _userRepository.GetWithRolesAndPermissionsByIdAsync(newUser.Id);
            return (userWithRoles ?? newUser).ToResponse();
        }

        public async Task<UserResponse> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetWithRolesAndPermissionsByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException(ApplicationConstants.ErrorMessages.UserNotFound);
            }
            return user.ToResponse();
        }

        public async Task LogoutAsync(string accessToken, string? refreshToken)
        {
            // 1. Thêm jti của Access Token vào InvalidatedTokens Blacklist
            var tokenInfo = JwtHelper.GetJtiAndExpiryFromToken(accessToken);
            if (tokenInfo.HasValue && !string.IsNullOrEmpty(tokenInfo.Value.Jti))
            {
                await _invalidatedTokenRepository.AddAsync(new InvalidatedTokenEntity
                {
                    Jti = tokenInfo.Value.Jti,
                    ExpiryTime = tokenInfo.Value.ExpiryTime
                });
                await _invalidatedTokenRepository.SaveChangesAsync();
            }

            // 2. Thu hồi Refresh Token nếu có
            if (!string.IsNullOrEmpty(refreshToken))
            {
                var storedToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken);
                if (storedToken != null)
                {
                    storedToken.IsRevoked = true;
                    storedToken.RevokedAt = DateTime.UtcNow;
                    await _refreshTokenRepository.SaveChangesAsync();
                }
            }
        }

        public async Task<AuthResponse> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var storedRefreshToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken);
            if (storedRefreshToken == null || storedRefreshToken.IsRevoked || storedRefreshToken.ExpiresAt <= DateTime.UtcNow)
            {
                throw new BadRequestException("Refresh token không hợp lệ hoặc đã hết hạn.");
            }

            // Revoke refresh token cũ (Rotate Token)
            storedRefreshToken.IsRevoked = true;
            storedRefreshToken.RevokedAt = DateTime.UtcNow;
            await _refreshTokenRepository.SaveChangesAsync();

            // Load user với full roles & permissions
            var user = await _userRepository.GetWithRolesAndPermissionsByIdAsync(storedRefreshToken.UserId);
            if (user == null)
            {
                throw new NotFoundException(ApplicationConstants.ErrorMessages.UserNotFound);
            }

            return await GenerateAuthResponseAsync(user);
        }

        private async Task<AuthResponse> GenerateAuthResponseAsync(UserEntity user)
        {
            var userResponse = user.ToResponse();

            var secretKey = _configuration["Jwt:SecretKey"] ?? "super_secret_jwt_key_microservice_2026_spec";
            var issuer = _configuration["Jwt:Issuer"] ?? "IdentityService.API";
            var audience = _configuration["Jwt:Audience"] ?? "IdentityService.Client";

            var jti = Guid.NewGuid().ToString();

            var accessToken = JwtHelper.GenerateToken(
                user,
                userResponse.Roles,
                userResponse.Permissions,
                jti,
                secretKey,
                issuer,
                audience,
                expirationMinutes: 480);

            var refreshTokenStr = Guid.NewGuid().ToString() + Guid.NewGuid().ToString();
            var refreshTokenEntity = new RefreshTokenEntity
            {
                UserId = user.Id,
                Token = refreshTokenStr,
                JwtId = jti,
                ExpiresAt = DateTime.UtcNow.AddDays(7),
                IsRevoked = false
            };

            await _refreshTokenRepository.AddAsync(refreshTokenEntity);
            await _refreshTokenRepository.SaveChangesAsync();

            return new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenStr,
                ExpiresIn = 28800,
                User = userResponse
            };
        }
    }
}

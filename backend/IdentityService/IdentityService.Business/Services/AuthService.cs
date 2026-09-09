using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using IdentityService.Business.Constants;
using IdentityService.Business.Dtos.Requests;
using IdentityService.Business.Dtos.Responses;
using IdentityService.Business.Exceptions;
using IdentityService.Business.Helpers;
using IdentityService.Business.Mappers;
using IdentityService.DataAccess.Entities;
using IdentityService.DataAccess.Repositories.Interfaces;

namespace IdentityService.Business.Services
{
    public interface IAuthService
    {
        Task<AuthResponse> LoginAsync(LoginRequest request);
        Task<UserResponse> RegisterAsync(RegisterRequest request);
        Task<UserResponse> GetProfileAsync(Guid userId);
    }

    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public AuthService(IUserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        public async Task<AuthResponse> LoginAsync(LoginRequest request)
        {
            var user = await _userRepository.GetByUsernameAsync(request.Username);
            if (user == null || !PasswordHasher.VerifyPassword(request.Password, user.PasswordHash))
            {
                throw new BadRequestException(ApplicationConstants.ErrorMessages.InvalidCredentials);
            }

            var secretKey = _configuration["Jwt:SecretKey"] ?? "super_secret_jwt_key_microservice_2026_spec";
            var issuer = _configuration["Jwt:Issuer"] ?? "IdentityService.API";
            var audience = _configuration["Jwt:Audience"] ?? "IdentityService.Client";

            var token = JwtHelper.GenerateToken(user, secretKey, issuer, audience);

            return new AuthResponse
            {
                AccessToken = token,
                RefreshToken = Guid.NewGuid().ToString(),
                ExpiresIn = 3600,
                User = user.ToResponse()
            };
        }

        public async Task<UserResponse> RegisterAsync(RegisterRequest request)
        {
            var existingUser = await _userRepository.GetByUsernameAsync(request.Username);
            if (existingUser != null)
            {
                throw new BadRequestException(ApplicationConstants.ErrorMessages.UsernameAlreadyExists);
            }

            var newUser = new UserEntity
            {
                Username = request.Username,
                Email = request.Email,
                PasswordHash = PasswordHasher.HashPassword(request.Password),
                FullName = request.FullName,
                Role = ApplicationConstants.Roles.User
            };

            await _userRepository.AddAsync(newUser);
            await _userRepository.SaveChangesAsync();

            return newUser.ToResponse();
        }

        public async Task<UserResponse> GetProfileAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new NotFoundException(ApplicationConstants.ErrorMessages.UserNotFound);
            }
            return user.ToResponse();
        }
    }
}

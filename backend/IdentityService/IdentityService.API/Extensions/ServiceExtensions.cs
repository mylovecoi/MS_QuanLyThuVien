using System.IdentityModel.Tokens.Jwt;
using System.Text;
using FluentValidation;
using IdentityService.API.Authorization;
using IdentityService.API.Configurations;
using IdentityService.Business.Clients;
using IdentityService.Business.Dtos.Requests;
using IdentityService.Business.Services;
using IdentityService.Business.Services.Implementations;
using IdentityService.Business.Services.Interfaces;
using IdentityService.Business.Validators;
using IdentityService.DataAccess.Contexts;
using IdentityService.DataAccess.Repositories.Implementations;
using IdentityService.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Shared.Contracts.Interfaces;

namespace IdentityService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Connection string bắt buộc: ConfigurationGuard.RequireSecrets chạy trước
            // trong Program.cs và chặn app nếu giá trị rỗng. Không có fallback InMemory
            // vì sẽ khiến dữ liệu dev biến mất khi restart mà không có cảnh báo.
            var connectionString = configuration.GetConnectionString("DefaultConnection")!;

            services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddRepositoriesAndServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Audit tự động: đọc UserId từ JWT token của mỗi HTTP request
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserContext, CurrentUserContext>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IInvalidatedTokenRepository, InvalidatedTokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IRoleService, RoleService>();

            // FluentValidation: đăng ký thủ công từng validator để khỏi thêm package
            // FluentValidation.DependencyInjectionExtensions. Validator được inject
            // vào service và chạy qua ValidationRunner.EnsureValidAsync.
            services.AddScoped<IValidator<LoginRequest>, LoginRequestValidator>();
            services.AddScoped<IValidator<RegisterRequest>, RegisterRequestValidator>();
            services.AddScoped<IValidator<RoleRequest>, RoleRequestValidator>();

            // HTTP Client gọi sang ProfileService
            services.AddHttpClient<IProfileServiceClient, ProfileServiceClient>(client =>
            {
                var baseUrl = configuration["ProfileService:BaseUrl"] ?? "http://localhost:5006";
                client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");

                // Timeout và số lần thử lại do ProfileServiceClient tự quản lý
                // (đọc từ ProfileService:TimeoutSeconds / MaxRetries) để có thể
                // retry khi service chưa kịp khởi động. Ở đây chỉ đặt fallback.
                client.Timeout = TimeSpan.FromSeconds(10);
            });

            // Permission Authorization
            services.AddSingleton<IAuthorizationPolicyProvider, PermissionPolicyProvider>();
            services.AddScoped<IAuthorizationHandler, PermissionAuthorizationHandler>();

            return services;
        }

        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            var jwtOptions = configuration.GetSection(JwtOptions.SectionName).Get<JwtOptions>() ?? new JwtOptions();
            var key = Encoding.UTF8.GetBytes(jwtOptions.SecretKey.PadRight(32, '0'));

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtOptions.Issuer,
                    ValidAudience = jwtOptions.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var invalidatedTokenRepo = context.HttpContext.RequestServices.GetRequiredService<IInvalidatedTokenRepository>();
                        var jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value
                               ?? context.Principal?.FindFirst("jti")?.Value;

                        if (!string.IsNullOrEmpty(jti))
                        {
                            var isInvalidated = await invalidatedTokenRepo.IsInvalidatedAsync(jti);
                            if (isInvalidated)
                            {
                                context.Fail("Access token này đã bị vô hiệu hóa do Logout.");
                            }
                        }
                    }
                };
            });

            return services;
        }
    }
}

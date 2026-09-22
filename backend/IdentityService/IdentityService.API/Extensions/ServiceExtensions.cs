using System.IdentityModel.Tokens.Jwt;
using System.Text;
using IdentityService.API.Authorization;
using IdentityService.API.Configurations;
using IdentityService.Business.Services;
using IdentityService.DataAccess.Contexts;
using IdentityService.DataAccess.Repositories.Implementations;
using IdentityService.DataAccess.Repositories.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace IdentityService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(connectionString))
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseSqlServer(connectionString));
            }
            else
            {
                services.AddDbContext<AppDbContext>(options =>
                    options.UseInMemoryDatabase("IdentityInMemoryDb"));
            }
            return services;
        }

        public static IServiceCollection AddRepositoriesAndServices(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddScoped<IInvalidatedTokenRepository, InvalidatedTokenRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<IAuthService, AuthService>();

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

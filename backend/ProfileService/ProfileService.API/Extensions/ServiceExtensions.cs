using System.Text;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using ProfileService.API.Configurations;
using ProfileService.API.Filters;
using ProfileService.Business.Dtos.Requests;
using ProfileService.Business.Services.Implementations;
using ProfileService.Business.Services.Interfaces;
using ProfileService.Business.Validators;
using ProfileServiceImpl = ProfileService.Business.Services.Implementations.ProfileService;
using ProfileService.DataAccess.Contexts;
using ProfileService.DataAccess.Repositories.Implementations;
using ProfileService.DataAccess.Repositories.Interfaces;
using Shared.Contracts.Interfaces;

namespace ProfileService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            // Connection string bắt buộc: ConfigurationGuard.RequireSecrets chạy trước
            // trong Program.cs và chặn app nếu giá trị rỗng. Không có fallback InMemory
            // vì sẽ khiến dữ liệu dev biến mất khi restart mà không có cảnh báo.
            var connectionString = configuration.GetConnectionString("DefaultConnection")!;

            services.AddDbContext<ProfileDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddRepositoriesAndServices(this IServiceCollection services)
        {
            // Audit tự động: đọc UserId từ JWT token của mỗi HTTP request
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserContext, CurrentUserContext>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IProfileRepository, ProfileRepository>();
            services.AddScoped<IPhongBanRepository, PhongBanRepository>();
            services.AddScoped<IChucVuRepository, ChucVuRepository>();

            services.AddScoped<IProfileService, ProfileServiceImpl>();
            services.AddScoped<IPhongBanService, PhongBanService>();
            services.AddScoped<IChucVuService, ChucVuService>();

            // FluentValidation: đăng ký thủ công từng validator để khỏi thêm package
            // FluentValidation.DependencyInjectionExtensions. Validator được inject
            // vào service và chạy qua ValidationRunner.EnsureValidAsync.
            services.AddScoped<IValidator<ProfileRequest>, ProfileRequestValidator>();
            services.AddScoped<IValidator<PhongBanRequest>, PhongBanRequestValidator>();
            services.AddScoped<IValidator<ChucVuRequest>, ChucVuRequestValidator>();

            return services;
        }

        public static IServiceCollection AddInternalApiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<InternalApiOptions>(
                configuration.GetSection(InternalApiOptions.SectionName));

            // Filter dùng IOptions + gọi qua DI, không thể AddAttribute cho
            // authorization filter nên dùng AddScoped cho ServiceFilter.
            services.AddScoped<InternalApiKeyFilter>();

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
            });

            return services;
        }
    }
}

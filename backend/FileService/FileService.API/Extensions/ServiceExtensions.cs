using System.Text;
using FileService.API.Configurations;
using FileService.Business.Dtos.Requests;
using FileService.Business.Services.Implementations;
using FileService.Business.Services.Interfaces;
using FileService.Business.Validators;
using FileService.DataAccess.Contexts;
using FileService.DataAccess.Repositories.Implementations;
using FileService.DataAccess.Repositories.Interfaces;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Shared.Contracts.Configuration;
using Shared.Contracts.Interfaces;

namespace FileService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection")!;

            services.AddDbContext<FileDbContext>(options =>
                options.UseSqlServer(connectionString));

            return services;
        }

        public static IServiceCollection AddRepositoriesAndServices(this IServiceCollection services, IConfiguration configuration)
        {
            var storageSection = configuration.GetSection(FileStorageSettings.SectionName);
            var storageSettings = storageSection.Get<FileStorageSettings>() ?? new FileStorageSettings();

            var uploadFolderPath = Path.IsPathRooted(storageSettings.UploadFolderPath)
                ? storageSettings.UploadFolderPath
                : Path.Combine(Directory.GetCurrentDirectory(), storageSettings.UploadFolderPath);

            services.AddSingleton<IStorageProvider>(new LocalStorageProvider(uploadFolderPath, storageSettings.BaseUrl));

            // Audit tự động: đọc UserId từ JWT token của mỗi HTTP request
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserContext, CurrentUserContext>();

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IAttachedFileRepository, AttachedFileRepository>();

            services.AddScoped<IFileService, FileServiceImpl>();

            // Register Validators manually
            services.AddScoped<IValidator<UploadFileRequest>, UploadFileRequestValidator>();
            services.AddScoped<IValidator<UpdateFileMetadataRequest>, UpdateFileMetadataRequestValidator>();

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

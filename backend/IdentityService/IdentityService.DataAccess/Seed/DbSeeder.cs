using IdentityService.DataAccess.Contexts;
using IdentityService.DataAccess.Entities;
using IdentityService.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DataAccess.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedDataAsync(AppDbContext context)
        {
            if (!await context.Users.AnyAsync())
            {
                var adminUser = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("admin123_secret_salt")), // Mật khẩu gốc: admin123
                    FullName = "System Administrator",
                    Gender = Gender.Male,
                    Status = AccountStatus.Active,
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();
            }
        }
    }
}

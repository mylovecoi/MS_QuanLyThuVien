using IdentityService.DataAccess.Contexts;
using IdentityService.DataAccess.Entities;
using IdentityService.DataAccess.Enums;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DataAccess.Seed
{
    public static class DbSeeder
    {
        public static async Task SeedDataAsync(AppDbContext context, Guid adminUserId)
        {
            // 1. Seed Permissions
            if (!await context.Permissions.AnyAsync())
            {
                var permissions = new List<PermissionEntity>
                {
                    new PermissionEntity { Code = "users.read", Name = "Xem người dùng", Module = "Users", Description = "Quyền xem danh sách và chi tiết người dùng" },
                    new PermissionEntity { Code = "users.write", Name = "Quản lý người dùng", Module = "Users", Description = "Quyền thêm, sửa, xóa người dùng" },
                    new PermissionEntity { Code = "roles.read", Name = "Xem vai trò", Module = "Roles", Description = "Quyền xem danh sách vai trò và quyền hạn" },
                    new PermissionEntity { Code = "roles.write", Name = "Quản lý vai trò", Module = "Roles", Description = "Quyền tạo, sửa, phân quyền cho vai trò" },
                    new PermissionEntity { Code = "profile.read", Name = "Xem hồ sơ cá nhân", Module = "Profile", Description = "Quyền xem thông tin cá nhân" },
                    new PermissionEntity { Code = "profile.write", Name = "Cập nhật hồ sơ cá nhân", Module = "Profile", Description = "Quyền cập nhật thông tin cá nhân" }
                };

                await context.Permissions.AddRangeAsync(permissions);
                await context.SaveChangesAsync();
            }

            // 2. Seed Roles & RolePermissions
            if (!await context.Roles.AnyAsync())
            {
                var allPermissions = await context.Permissions.ToListAsync();

                var adminRole = new RoleEntity
                {
                    Code = "ADMIN",
                    Name = "Quản trị hệ thống",
                    Description = "Toàn quyền trên hệ thống"
                };

                var managerRole = new RoleEntity
                {
                    Code = "MANAGER",
                    Name = "Quản lý",
                    Description = "Quản lý thông tin người dùng và xem vai trò"
                };

                var userRole = new RoleEntity
                {
                    Code = "USER",
                    Name = "Người dùng",
                    Description = "Người dùng mặc định"
                };

                await context.Roles.AddRangeAsync(adminRole, managerRole, userRole);
                await context.SaveChangesAsync();

                // Gán permissions cho Roles
                var rolePermissions = new List<RolePermissionEntity>();

                // Admin: All permissions
                foreach (var perm in allPermissions)
                {
                    rolePermissions.Add(new RolePermissionEntity { RoleId = adminRole.Id, PermissionId = perm.Id });
                }

                // Manager: users.read, roles.read, profile.read, profile.write
                var managerPerms = allPermissions.Where(p => p.Code is "users.read" or "roles.read" or "profile.read" or "profile.write");
                foreach (var perm in managerPerms)
                {
                    rolePermissions.Add(new RolePermissionEntity { RoleId = managerRole.Id, PermissionId = perm.Id });
                }

                // User: profile.read, profile.write
                var userPerms = allPermissions.Where(p => p.Code is "profile.read" or "profile.write");
                foreach (var perm in userPerms)
                {
                    rolePermissions.Add(new RolePermissionEntity { RoleId = userRole.Id, PermissionId = perm.Id });
                }

                await context.RolePermissions.AddRangeAsync(rolePermissions);
                await context.SaveChangesAsync();
            }

            // 3. Seed Admin User & Assign ADMIN role
            if (!await context.Users.AnyAsync())
            {
                var adminRole = await context.Roles.FirstOrDefaultAsync(r => r.Code == "ADMIN");

                var adminUser = new UserEntity
                {
                    Id = adminUserId,
                    Username = "admin",
                    Email = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"), // Mật khẩu gốc: admin
                    FullName = "System Administrator",
                    Gender = Gender.Male,
                    Status = AccountStatus.Active,
                    Role = "Admin",
                    CreatedAt = DateTime.UtcNow
                };

                await context.Users.AddAsync(adminUser);
                await context.SaveChangesAsync();

                if (adminRole != null)
                {
                    await context.UserRoles.AddAsync(new UserRoleEntity
                    {
                        UserId = adminUser.Id,
                        RoleId = adminRole.Id
                    });
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}

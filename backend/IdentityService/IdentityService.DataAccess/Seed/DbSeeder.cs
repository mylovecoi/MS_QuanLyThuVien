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
                var permissions = new List<PermissionEntity>();

                var moduleDefinitions = new (string Module, string Label, string[] Actions)[]
                {
                    ("Catalog",      "Danh mục thư mục",      ["index", "create", "edit", "delete", "approve"]),
                    ("Inventory",    "Kho & Bản sao",         ["index", "create", "edit", "delete"]),
                    ("Circulation",  "Mượn trả sách",         ["index", "create", "edit", "delete", "approve"]),
                    ("Acquisition",  "Bổ sung tài liệu",      ["index", "create", "edit", "delete", "approve"]),
                    ("Review",       "Đánh giá & Bình luận",  ["index", "create", "edit", "delete"]),
                    ("Notification", "Thông báo",             ["index", "create", "edit", "delete"]),
                    ("Search",       "Tìm kiếm",              ["index"]),
                    ("Reporting",    "Báo cáo & Thống kê",    ["index", "create"]),
                    ("Users",        "Người dùng",            ["index", "create", "edit", "delete"]),
                    ("Roles",        "Vai trò & Phân quyền",  ["index", "create", "edit", "delete"]),
                };

                foreach (var (module, label, actions) in moduleDefinitions)
                {
                    foreach (var action in actions)
                    {
                        permissions.Add(new PermissionEntity
                        {
                            Code        = $"{module.ToLower()}.{action}",
                            Name        = $"{GetActionLabel(action)} {label}",
                            Module      = module,
                            Description = $"Quyền {GetActionLabel(action).ToLower()} dữ liệu thuộc {label}"
                        });
                    }
                }

                await context.Permissions.AddRangeAsync(permissions);
                await context.SaveChangesAsync();
            }

            // 2. Seed Roles & RolePermissions
            if (!await context.Roles.AnyAsync())
            {
                var allPermissions = await context.Permissions.ToListAsync();

                var adminRole = new RoleEntity
                {
                    Code        = "ADMIN",
                    Name        = "Quản trị hệ thống",
                    Description = "Toàn quyền trên hệ thống"
                };

                var librarianRole = new RoleEntity
                {
                    Code        = "LIBRARIAN",
                    Name        = "Thủ thư",
                    Description = "Quản lý catalog, kho sách và quy trình mượn trả"
                };

                var managerRole = new RoleEntity
                {
                    Code        = "MANAGER",
                    Name        = "Trưởng phòng",
                    Description = "Phê duyệt quy trình, xem báo cáo và thống kê"
                };

                var userRole = new RoleEntity
                {
                    Code        = "USER",
                    Name        = "Bạn đọc",
                    Description = "Người dùng thư viện thông thường"
                };

                await context.Roles.AddRangeAsync(adminRole, librarianRole, managerRole, userRole);
                await context.SaveChangesAsync();

                var rolePermissions = new List<RolePermissionEntity>();

                // ADMIN: Tất cả 38 quyền
                foreach (var perm in allPermissions)
                {
                    rolePermissions.Add(new RolePermissionEntity { RoleId = adminRole.Id, PermissionId = perm.Id });
                }

                // LIBRARIAN: Nghiệp vụ thư viện chính, không có approve và không quản lý roles/users
                var librarianCodes = new HashSet<string>
                {
                    "catalog.index", "catalog.create", "catalog.edit", "catalog.delete",
                    "inventory.index", "inventory.create", "inventory.edit", "inventory.delete",
                    "circulation.index", "circulation.create", "circulation.edit", "circulation.delete",
                    "acquisition.index", "acquisition.create", "acquisition.edit",
                    "review.index", "review.delete",
                    "notification.index",
                    "search.index",
                    "reporting.index",
                    "users.index",
                };
                foreach (var perm in allPermissions.Where(p => librarianCodes.Contains(p.Code)))
                {
                    rolePermissions.Add(new RolePermissionEntity { RoleId = librarianRole.Id, PermissionId = perm.Id });
                }

                // MANAGER: Phê duyệt + báo cáo + xem tổng quan
                var managerCodes = new HashSet<string>
                {
                    "catalog.index", "catalog.approve",
                    "inventory.index",
                    "circulation.index", "circulation.approve",
                    "acquisition.index", "acquisition.approve",
                    "review.index",
                    "notification.index", "notification.create",
                    "search.index",
                    "reporting.index", "reporting.create",
                    "users.index",
                    "roles.index",
                };
                foreach (var perm in allPermissions.Where(p => managerCodes.Contains(p.Code)))
                {
                    rolePermissions.Add(new RolePermissionEntity { RoleId = managerRole.Id, PermissionId = perm.Id });
                }

                // USER (Bạn đọc): Quyền cơ bản — xem, tìm kiếm, đánh giá
                var userCodes = new HashSet<string>
                {
                    "catalog.index",
                    "inventory.index",
                    "circulation.index",
                    "review.index", "review.create", "review.edit", "review.delete",
                    "search.index",
                };
                foreach (var perm in allPermissions.Where(p => userCodes.Contains(p.Code)))
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
                    Id           = adminUserId,
                    Username     = "admin",
                    Email        = "admin@example.com",
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin"), // Mật khẩu gốc: admin
                    FullName     = "System Administrator",
                    Gender       = Gender.Male,
                    Status       = AccountStatus.Active,
                    Role         = "Admin",
                    CreatedAt    = DateTime.UtcNow
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

        /// <summary>
        /// Chuyển action code sang nhãn tiếng Việt dùng khi tạo tên permission.
        /// </summary>
        private static string GetActionLabel(string action) => action switch
        {
            "index"   => "Xem danh sách",
            "create"  => "Tạo mới",
            "edit"    => "Chỉnh sửa",
            "delete"  => "Xóa",
            "approve" => "Phê duyệt",
            _         => action
        };
    }
}

using System.Reflection;
using IdentityService.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Shared.Contracts.Interfaces;

namespace IdentityService.DataAccess.Contexts
{
    public class AppDbContext : DbContext
    {
        private readonly ICurrentUserContext? _currentUser;

        public AppDbContext(DbContextOptions<AppDbContext> options, ICurrentUserContext? currentUser = null)
            : base(options)
        {
            _currentUser = currentUser;
        }

        public DbSet<UserEntity> Users => Set<UserEntity>();
        public DbSet<RoleEntity> Roles => Set<RoleEntity>();
        public DbSet<PermissionEntity> Permissions => Set<PermissionEntity>();
        public DbSet<UserRoleEntity> UserRoles => Set<UserRoleEntity>();
        public DbSet<RolePermissionEntity> RolePermissions => Set<RolePermissionEntity>();
        public DbSet<RefreshTokenEntity> RefreshTokens => Set<RefreshTokenEntity>();
        public DbSet<InvalidatedTokenEntity> InvalidatedTokens => Set<InvalidatedTokenEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// Tự động điền CreatedAt, CreatedBy khi thêm mới
        /// và UpdatedAt, UpdatedBy khi cập nhật / xoá mềm,
        /// đọc UserId trực tiếp từ JWT token của request hiện tại.
        /// </summary>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var now = DateTime.UtcNow;
            var currentUserId = _currentUser?.UserId;

            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = now;
                    if (currentUserId.HasValue)
                        entry.Entity.CreatedBy = currentUserId.Value;
                }
                else if (entry.State == EntityState.Modified)
                {
                    // Giữ nguyên CreatedAt và CreatedBy gốc, không cho phép sửa
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;

                    entry.Entity.UpdatedAt = now;
                    if (currentUserId.HasValue)
                        entry.Entity.UpdatedBy = currentUserId.Value;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}

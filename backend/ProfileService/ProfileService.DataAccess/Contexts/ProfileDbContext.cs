using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ProfileService.DataAccess.Entities;
using Shared.Contracts.Interfaces;

namespace ProfileService.DataAccess.Contexts
{
    public class ProfileDbContext : DbContext
    {
        private readonly ICurrentUserContext? _currentUser;

        public ProfileDbContext(DbContextOptions<ProfileDbContext> options, ICurrentUserContext? currentUser = null)
            : base(options)
        {
            _currentUser = currentUser;
        }

        public DbSet<ProfileEntity> Profiles => Set<ProfileEntity>();
        public DbSet<PhongBanEntity> PhongBans => Set<PhongBanEntity>();
        public DbSet<ChucVuEntity> ChucVus => Set<ChucVuEntity>();

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

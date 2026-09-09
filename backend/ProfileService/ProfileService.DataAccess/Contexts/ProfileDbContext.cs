using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ProfileService.DataAccess.Entities;

namespace ProfileService.DataAccess.Contexts
{
    public class ProfileDbContext : DbContext
    {
        public ProfileDbContext(DbContextOptions<ProfileDbContext> options) : base(options)
        {
        }

        public DbSet<UserProfileEntity> UserProfiles => Set<UserProfileEntity>();
        public DbSet<PhongBanEntity> PhongBans => Set<PhongBanEntity>();
        public DbSet<ChucVuEntity> ChucVus => Set<ChucVuEntity>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IdentityService.DataAccess.Entities;

namespace IdentityService.DataAccess.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserEntity>
    {
        public void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.HasKey(u => u.Id);
            builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
            builder.HasIndex(u => u.Username).IsUnique();
            builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
            builder.HasIndex(u => u.Email).IsUnique();
            builder.Property(u => u.FullName).HasMaxLength(100);
            builder.Property(u => u.Role).HasMaxLength(20);
        }
    }
}

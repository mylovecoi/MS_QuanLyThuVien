using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using IdentityService.DataAccess.Entities;

namespace IdentityService.DataAccess.Configurations
{
    public class InvalidatedTokenConfiguration : IEntityTypeConfiguration<InvalidatedTokenEntity>
    {
        public void Configure(EntityTypeBuilder<InvalidatedTokenEntity> builder)
        {
            builder.HasKey(it => it.Id);
            builder.Property(it => it.Jti).IsRequired().HasMaxLength(100);
            builder.HasIndex(it => it.Jti).IsUnique();
        }
    }
}

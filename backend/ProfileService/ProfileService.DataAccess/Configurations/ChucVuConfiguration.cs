using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.DataAccess.Entities;

namespace ProfileService.DataAccess.Configurations
{
    public class ChucVuConfiguration : IEntityTypeConfiguration<ChucVuEntity>
    {
        public void Configure(EntityTypeBuilder<ChucVuEntity> builder)
        {
            builder.ToTable("ChucVus");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.MaChucVu).HasMaxLength(50).IsRequired();
            builder.Property(x => x.TenChucVu).HasMaxLength(200).IsRequired();
        }
    }
}

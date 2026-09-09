using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.DataAccess.Entities;

namespace ProfileService.DataAccess.Configurations
{
    public class PhongBanConfiguration : IEntityTypeConfiguration<PhongBanEntity>
    {
        public void Configure(EntityTypeBuilder<PhongBanEntity> builder)
        {
            builder.ToTable("PhongBans");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.MaPhongBan).HasMaxLength(50).IsRequired();
            builder.Property(x => x.TenPhongBan).HasMaxLength(200).IsRequired();
            builder.Property(x => x.TreePath).HasMaxLength(500);

            builder.HasOne(x => x.Parent)
                   .WithMany(x => x.Children)
                   .HasForeignKey(x => x.ParentId)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}

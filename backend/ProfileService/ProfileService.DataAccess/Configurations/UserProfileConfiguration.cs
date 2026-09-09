using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ProfileService.DataAccess.Entities;

namespace ProfileService.DataAccess.Configurations
{
    public class UserProfileConfiguration : IEntityTypeConfiguration<UserProfileEntity>
    {
        public void Configure(EntityTypeBuilder<UserProfileEntity> builder)
        {
            builder.ToTable("UserProfiles");
            builder.HasKey(x => x.Id);

            // Indexes
            builder.HasIndex(x => x.UserId).IsUnique();

            // Length limits
            builder.Property(x => x.MaCanBo).HasMaxLength(50);
            builder.Property(x => x.FullName).HasMaxLength(200).IsRequired();
            builder.Property(x => x.Cccd).HasMaxLength(20);
            builder.Property(x => x.PhoneNumber).HasMaxLength(20);
            builder.Property(x => x.CoQuanPhoneNumber).HasMaxLength(20);
            builder.Property(x => x.WorkEmail).HasMaxLength(150);
            builder.Property(x => x.AvatarUrl).HasMaxLength(500);
            builder.Property(x => x.DiaChiHienNay).HasMaxLength(500);
            builder.Property(x => x.TenPhongBan).HasMaxLength(200);
            builder.Property(x => x.TenChucVu).HasMaxLength(200);

            // Relationships
            builder.HasOne(x => x.PhongBan)
                   .WithMany(pb => pb.UserProfiles)
                   .HasForeignKey(x => x.PhongBanId)
                   .OnDelete(DeleteBehavior.SetNull);

            builder.HasOne(x => x.ChucVu)
                   .WithMany(cv => cv.UserProfiles)
                   .HasForeignKey(x => x.ChucVuId)
                   .OnDelete(DeleteBehavior.SetNull);
        }
    }
}

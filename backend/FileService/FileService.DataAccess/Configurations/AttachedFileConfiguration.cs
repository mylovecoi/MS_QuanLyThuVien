using FileService.DataAccess.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FileService.DataAccess.Configurations
{
    public class AttachedFileConfiguration : IEntityTypeConfiguration<AttachedFileEntity>
    {
        public void Configure(EntityTypeBuilder<AttachedFileEntity> builder)
        {
            builder.ToTable("AttachedFiles");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Url).IsRequired();
            builder.Property(x => x.FileName).IsRequired();
            builder.Property(x => x.ContentType).IsRequired();
            builder.Property(x => x.MoTa).IsRequired(false);
            
            // Map BaseEntity audit properties to SQL Table column names
            builder.Property(x => x.CreatedAt).HasColumnName("CreatedDate").IsRequired();
            builder.Property(x => x.CreatedBy).HasColumnName("CreatedBy").IsRequired();
            builder.Property(x => x.UpdatedAt).HasColumnName("UpdatedDate");
            builder.Property(x => x.UpdatedBy).HasColumnName("UpdatedBy").IsRequired();

            builder.Property(x => x.FileSize).HasDefaultValue(0);
            builder.Property(x => x.LoaiFile).HasDefaultValue(0);
            builder.Property(x => x.Sha256).HasMaxLength(64).HasDefaultValue(string.Empty);

            builder.HasQueryFilter(x => !x.IsDeleted);
        }
    }
}

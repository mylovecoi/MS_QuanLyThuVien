using ProfileService.DataAccess.Enums;

namespace ProfileService.DataAccess.Entities
{
    public class ProfileEntity : BaseEntity
    {
        public Guid UserId { get; set; }            // FK to IdentityService.UserEntity.Id
        public string MaCanBo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Cccd { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public Gender Gender { get; set; } = Gender.Unknown;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CoQuanPhoneNumber { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string DiaChiHienNay { get; set; } = string.Empty;
        public Guid? PhongBanId { get; set; }       // FK to PhongBan
        public string TenPhongBan { get; set; } = string.Empty;
        public Guid? ChucVuId { get; set; }         // FK to ChucVu
        public string TenChucVu { get; set; } = string.Empty;

        // Navigation properties:
        public virtual PhongBanEntity? PhongBan { get; set; }
        public virtual ChucVuEntity? ChucVu { get; set; }
    }
}

using ProfileService.DataAccess.Enums;

namespace ProfileService.Business.Dtos.Responses
{
    public class ProfileDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string MaCanBo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Cccd { get; set; } = string.Empty;
        public DateTime? NgaySinh { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; } = string.Empty;
        public string CoQuanPhoneNumber { get; set; } = string.Empty;
        public string WorkEmail { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string DiaChiHienNay { get; set; } = string.Empty;
        public Guid? PhongBanId { get; set; }
        public string TenPhongBan { get; set; } = string.Empty;
        public Guid? ChucVuId { get; set; }
        public string TenChucVu { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}

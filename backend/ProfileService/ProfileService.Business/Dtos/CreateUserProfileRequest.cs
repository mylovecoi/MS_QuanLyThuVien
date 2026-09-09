using System;
using ProfileService.DataAccess.Enums;

namespace ProfileService.Business.Dtos
{
    public class CreateUserProfileRequest
    {
        public Guid UserId { get; set; }
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
        public Guid? PhongBanId { get; set; }
        public Guid? ChucVuId { get; set; }
    }

    public class UpdateUserProfileRequest
    {
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
        public Guid? ChucVuId { get; set; }
    }

    public class CreatePhongBanRequest
    {
        public string MaPhongBan { get; set; } = string.Empty;
        public string TenPhongBan { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public int OrderNo { get; set; } = 0;
    }

    public class CreateChucVuRequest
    {
        public string MaChucVu { get; set; } = string.Empty;
        public string TenChucVu { get; set; } = string.Empty;
        public int CapBac { get; set; } = 0;
    }
}

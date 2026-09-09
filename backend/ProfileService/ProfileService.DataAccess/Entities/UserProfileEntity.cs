using System;
using ProfileService.DataAccess.Enums;

namespace ProfileService.DataAccess.Entities
{
    public class UserProfileEntity : BaseEntity
    {
        // 1. Khóa ngoại liên kết với Identity Service (Cross-Service Loose Reference)
        public Guid UserId { get; set; } // Tham chiếu tới IdentityService.UserEntity.Id

        // 2. Định danh Cán bộ / Nhân sự
        public string MaCanBo { get; set; } = string.Empty;       // Mã cán bộ / Mã nhân viên
        public string FullName { get; set; } = string.Empty;      // Họ và tên
        public string Cccd { get; set; } = string.Empty;           // Căn cước công dân
        public DateTime? NgaySinh { get; set; }
        public Gender Gender { get; set; } = Gender.Unknown;

        // 3. Liên hệ Công vụ & Cá nhân
        public string PhoneNumber { get; set; } = string.Empty;   // SĐT di động
        public string CoQuanPhoneNumber { get; set; } = string.Empty; // SĐT máy bàn / máy lẻ
        public string WorkEmail { get; set; } = string.Empty;     // Email công vụ (.gov.vn)
        public string AvatarUrl { get; set; } = string.Empty;     // Ảnh đại diện / Chân dung
        public string DiaChiHienNay { get; set; } = string.Empty;  // Nơi ở hiện tại

        // 4. Tổ chức, Phòng ban & Chức vụ
        public Guid? PhongBanId { get; set; }                      // ID Phòng / Ban
        public string TenPhongBan { get; set; } = string.Empty;    // Cache tên phòng (tránh join nhiều)
        
        public Guid? ChucVuId { get; set; }                        // ID Chức vụ
        public string TenChucVu { get; set; } = string.Empty;      // Cache tên chức vụ

        // EF Core Navigation Properties
        public virtual PhongBanEntity? PhongBan { get; set; }
        public virtual ChucVuEntity? ChucVu { get; set; }
    }
}

namespace ProfileService.DataAccess.Entities
{
    public class ChucVuEntity : BaseEntity
    {
        public string MaChucVu { get; set; } = string.Empty;      // Mã chức vụ
        public string TenChucVu { get; set; } = string.Empty;     // Tên chức vụ
        public int CapBac { get; set; } = 0;                      // Độ ưu tiên / Cấp bậc (1: Trưởng, 2: Phó...)
        public bool IsActive { get; set; } = true;                // Trạng thái hoạt động

        // Navigation Properties
        public virtual ICollection<ProfileEntity> Profiles { get; set; } = new List<ProfileEntity>();
    }
}

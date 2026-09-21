namespace ProfileService.DataAccess.Entities
{
    public class PhongBanEntity : BaseEntity
    {
        public string MaPhongBan { get; set; } = string.Empty;    // Mã phòng ban
        public string TenPhongBan { get; set; } = string.Empty;   // Tên phòng ban
        public Guid? ParentId { get; set; }                       // Cấp cha (Sở -> Chi cục -> Phòng)
        public string TreePath { get; set; } = string.Empty;      // Đường dẫn cây tổ chức: /parentId/id
        public int OrderNo { get; set; } = 0;                     // Thứ tự sắp xếp
        public bool IsActive { get; set; } = true;                // Trạng thái hoạt động

        // Navigation Properties
        public virtual PhongBanEntity? Parent { get; set; }
        public virtual ICollection<PhongBanEntity> Children { get; set; } = new List<PhongBanEntity>();
        public virtual ICollection<ProfileEntity> Profiles { get; set; } = new List<ProfileEntity>();
    }
}

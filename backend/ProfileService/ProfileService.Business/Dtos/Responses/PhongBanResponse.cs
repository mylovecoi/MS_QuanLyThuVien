namespace ProfileService.Business.Dtos.Responses
{
    public class PhongBanDto
    {
        public Guid Id { get; set; }
        public string MaPhongBan { get; set; } = string.Empty;
        public string TenPhongBan { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public string TreePath { get; set; } = string.Empty;
        public int OrderNo { get; set; }
        public bool IsActive { get; set; }
    }
}

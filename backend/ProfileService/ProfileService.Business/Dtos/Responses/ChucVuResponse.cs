namespace ProfileService.Business.Dtos.Responses
{
    public class ChucVuDto
    {
        public Guid Id { get; set; }
        public string MaChucVu { get; set; } = string.Empty;
        public string TenChucVu { get; set; } = string.Empty;
        public int CapBac { get; set; }
        public bool IsActive { get; set; }
    }
}

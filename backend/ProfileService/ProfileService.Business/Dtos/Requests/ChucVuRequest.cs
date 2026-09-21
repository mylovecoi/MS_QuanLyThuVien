namespace ProfileService.Business.Dtos.Requests
{
    public class ChucVuRequest
    {
        public string MaChucVu { get; set; } = string.Empty;
        public string TenChucVu { get; set; } = string.Empty;
        public int CapBac { get; set; } = 0;
    }
}

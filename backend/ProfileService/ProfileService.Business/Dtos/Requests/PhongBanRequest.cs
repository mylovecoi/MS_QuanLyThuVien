using System;

namespace ProfileService.Business.Dtos.Requests
{
    public class PhongBanRequest
    {
        public string MaPhongBan { get; set; } = string.Empty;
        public string TenPhongBan { get; set; } = string.Empty;
        public Guid? ParentId { get; set; }
        public int OrderNo { get; set; } = 0;
    }
}

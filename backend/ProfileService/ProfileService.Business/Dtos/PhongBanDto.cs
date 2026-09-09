using System;
using System.Collections.Generic;

namespace ProfileService.Business.Dtos
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

    public class ChucVuDto
    {
        public Guid Id { get; set; }
        public string MaChucVu { get; set; } = string.Empty;
        public string TenChucVu { get; set; } = string.Empty;
        public int CapBac { get; set; }
        public bool IsActive { get; set; }
    }
}

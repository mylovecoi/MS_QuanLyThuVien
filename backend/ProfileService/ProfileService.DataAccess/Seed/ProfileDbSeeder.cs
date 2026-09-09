using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProfileService.DataAccess.Contexts;
using ProfileService.DataAccess.Entities;
using ProfileService.DataAccess.Enums;

namespace ProfileService.DataAccess.Seed
{
    public static class ProfileDbSeeder
    {
        public static async Task SeedDataAsync(ProfileDbContext context)
        {
            if (await context.PhongBans.AnyAsync())
            {
                return; // Seeded already
            }

            // 1. Seed Phong Ban
            var phongBanTongHop = new PhongBanEntity
            {
                Id = Guid.NewGuid(),
                MaPhongBan = "PB-TONGHOP",
                TenPhongBan = "Phòng Tổng hợp & Hành chính",
                OrderNo = 1,
                TreePath = "/root"
            };

            var phongBanCNTT = new PhongBanEntity
            {
                Id = Guid.NewGuid(),
                MaPhongBan = "PB-CNTT",
                TenPhongBan = "Phòng Công nghệ thông tin",
                OrderNo = 2,
                TreePath = "/root"
            };

            await context.PhongBans.AddRangeAsync(phongBanTongHop, phongBanCNTT);

            // 2. Seed Chuc Vu
            var chucVuTruongPhong = new ChucVuEntity
            {
                Id = Guid.NewGuid(),
                MaChucVu = "CV-TP",
                TenChucVu = "Trưởng phòng",
                CapBac = 1
            };

            var chucVuChuyenVien = new ChucVuEntity
            {
                Id = Guid.NewGuid(),
                MaChucVu = "CV-CV",
                TenChucVu = "Chuyên viên",
                CapBac = 2
            };

            await context.ChucVus.AddRangeAsync(chucVuTruongPhong, chucVuChuyenVien);

            await context.SaveChangesAsync();
        }
    }
}

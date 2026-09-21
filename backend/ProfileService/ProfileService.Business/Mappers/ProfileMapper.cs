using ProfileService.Business.Dtos.Responses;
using ProfileService.DataAccess.Entities;

namespace ProfileService.Business.Mappers
{
    public static class ProfileMapper
    {
        public static ProfileDto ToDto(ProfileEntity entity)
        {
            return new ProfileDto
            {
                Id = entity.Id,
                UserId = entity.UserId,
                MaCanBo = entity.MaCanBo,
                FullName = entity.FullName,
                Cccd = entity.Cccd,
                NgaySinh = entity.NgaySinh,
                Gender = entity.Gender,
                PhoneNumber = entity.PhoneNumber,
                CoQuanPhoneNumber = entity.CoQuanPhoneNumber,
                WorkEmail = entity.WorkEmail,
                AvatarUrl = entity.AvatarUrl,
                DiaChiHienNay = entity.DiaChiHienNay,
                PhongBanId = entity.PhongBanId,
                TenPhongBan = entity.TenPhongBan,
                ChucVuId = entity.ChucVuId,
                TenChucVu = entity.TenChucVu,
                CreatedAt = entity.CreatedAt,
                UpdatedAt = entity.UpdatedAt
            };
        }

        public static PhongBanDto ToDto(PhongBanEntity entity)
        {
            return new PhongBanDto
            {
                Id = entity.Id,
                MaPhongBan = entity.MaPhongBan,
                TenPhongBan = entity.TenPhongBan,
                ParentId = entity.ParentId,
                TreePath = entity.TreePath,
                OrderNo = entity.OrderNo,
                IsActive = entity.IsActive
            };
        }

        public static ChucVuDto ToDto(ChucVuEntity entity)
        {
            return new ChucVuDto
            {
                Id = entity.Id,
                MaChucVu = entity.MaChucVu,
                TenChucVu = entity.TenChucVu,
                CapBac = entity.CapBac,
                IsActive = entity.IsActive
            };
        }
    }
}

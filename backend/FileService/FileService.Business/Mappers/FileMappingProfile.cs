using FileService.Business.Dtos.Responses;
using FileService.Business.Enums;
using FileService.DataAccess.Entities;

namespace FileService.Business.Mappers
{
    public static class FileMappingProfile
    {
        public static AttachedFileResponse ToResponse(this AttachedFileEntity entity)
        {
            return new AttachedFileResponse
            {
                Id = entity.Id,
                Url = entity.Url,
                FileName = entity.FileName,
                ContentType = entity.ContentType,
                MoTa = entity.MoTa,
                FileSize = entity.FileSize,
                LoaiFile = entity.LoaiFile,
                LoaiFileText = GetLoaiFileText(entity.LoaiFile),
                Sha256 = entity.Sha256,
                CreatedBy = entity.CreatedBy,
                CreatedDate = entity.CreatedAt,
                UpdatedBy = entity.UpdatedBy,
                UpdatedDate = entity.UpdatedAt ?? entity.CreatedAt
            };
        }

        public static string GetLoaiFileText(int loaiFile)
        {
            return (LoaiFileEnum)loaiFile switch
            {
                LoaiFileEnum.TaiLieu => "Tài liệu",
                LoaiFileEnum.HinhAnh => "Hình ảnh",
                LoaiFileEnum.Media => "Audio / Video",
                LoaiFileEnum.Archive => "Tệp nén",
                _ => "Khác"
            };
        }
    }
}

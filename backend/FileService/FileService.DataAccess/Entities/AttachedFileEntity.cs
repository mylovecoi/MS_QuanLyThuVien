namespace FileService.DataAccess.Entities
{
    public class AttachedFileEntity : BaseEntity
    {
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public long FileSize { get; set; } = 0;
        public int LoaiFile { get; set; } = 0;
        public string Sha256 { get; set; } = string.Empty;
    }
}

using System;

namespace FileService.Business.Dtos.Responses
{
    public class AttachedFileResponse
    {
        public Guid Id { get; set; }
        public string Url { get; set; } = string.Empty;
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
        public string? MoTa { get; set; }
        public long FileSize { get; set; }
        public int LoaiFile { get; set; }
        public string LoaiFileText { get; set; } = string.Empty;
        public string Sha256 { get; set; } = string.Empty;
        public Guid CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public Guid UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

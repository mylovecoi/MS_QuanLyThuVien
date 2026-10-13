using Microsoft.AspNetCore.Http;

namespace FileService.Business.Dtos.Requests
{
    public class UploadFileRequest
    {
        public required IFormFile File { get; set; }
        public string? MoTa { get; set; }
        public int LoaiFile { get; set; } = 0;
    }
}

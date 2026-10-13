namespace FileService.Business.Dtos.Requests
{
    public class UpdateFileMetadataRequest
    {
        public string? FileName { get; set; }
        public string? MoTa { get; set; }
        public int? LoaiFile { get; set; }
    }
}

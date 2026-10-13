namespace FileService.Business.Dtos.Requests
{
    public class FileFilterRequest
    {
        public string? Keyword { get; set; }
        public int? LoaiFile { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}

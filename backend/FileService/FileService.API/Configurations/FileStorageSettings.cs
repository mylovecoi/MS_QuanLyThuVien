namespace FileService.API.Configurations
{
    public class FileStorageSettings
    {
        public const string SectionName = "FileStorage";

        public string UploadFolderPath { get; set; } = "wwwroot/uploads";
        public string BaseUrl { get; set; } = "/uploads";
    }
}

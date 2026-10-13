using Microsoft.AspNetCore.Http;

namespace FileService.Business.Services.Interfaces
{
    public interface IStorageProvider
    {
        Task<(string Url, string RelativePath, string Sha256)> SaveFileAsync(IFormFile file);
        Task DeleteFileAsync(string relativePath);
        Task<(Stream Stream, string ContentType, string DownloadFileName)> GetFileStreamAsync(string relativePath, string originalFileName, string contentType);
    }
}

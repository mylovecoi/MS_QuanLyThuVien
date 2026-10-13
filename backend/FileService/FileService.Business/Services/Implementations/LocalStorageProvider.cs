using System.Security.Cryptography;
using FileService.Business.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Shared.Contracts.Exceptions;

namespace FileService.Business.Services.Implementations
{
    public class LocalStorageProvider : IStorageProvider
    {
        private readonly string _uploadFolderPath;
        private readonly string _baseUrl;

        public LocalStorageProvider(string uploadFolderPath, string baseUrl = "/uploads")
        {
            _uploadFolderPath = uploadFolderPath;
            _baseUrl = baseUrl.TrimEnd('/');

            if (!Directory.Exists(_uploadFolderPath))
            {
                Directory.CreateDirectory(_uploadFolderPath);
            }
        }

        public async Task<(string Url, string RelativePath, string Sha256)> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new BadRequestException("Tập tin tải lên rỗng.");
            }

            // Tính SHA256 Hash
            string sha256Hash;
            using (var sha256 = SHA256.Create())
            using (var stream = file.OpenReadStream())
            {
                var hashBytes = await sha256.ComputeHashAsync(stream);
                sha256Hash = Convert.ToHexString(hashBytes).ToLowerInvariant();
            }

            var extension = Path.GetExtension(file.FileName);
            var dateSubFolder = DateTime.UtcNow.ToString("yyyy/MM/dd");
            var targetFolder = Path.Combine(_uploadFolderPath, dateSubFolder);

            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }

            var uniqueFileName = $"{Guid.NewGuid():N}{extension}";
            var fullPath = Path.Combine(targetFolder, uniqueFileName);
            var relativePath = Path.Combine(dateSubFolder, uniqueFileName).Replace('\\', '/');

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = $"{_baseUrl}/{relativePath}";
            return (fileUrl, relativePath, sha256Hash);
        }

        public Task DeleteFileAsync(string relativePath)
        {
            if (string.IsNullOrWhiteSpace(relativePath)) return Task.CompletedTask;

            var fullPath = Path.Combine(_uploadFolderPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
            if (File.Exists(fullPath))
            {
                try
                {
                    File.Delete(fullPath);
                }
                catch
                {
                    // Ignore file deletion error if file in use
                }
            }

            return Task.CompletedTask;
        }

        public Task<(Stream Stream, string ContentType, string DownloadFileName)> GetFileStreamAsync(
            string relativePath, string originalFileName, string contentType)
        {
            var fullPath = Path.Combine(_uploadFolderPath, relativePath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(fullPath))
            {
                throw new NotFoundException($"Tập tin '{originalFileName}' không tồn tại trên hệ thống lưu trữ.");
            }

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return Task.FromResult<(Stream, string, string)>((stream, contentType, originalFileName));
        }
    }
}

using FileService.Business.Dtos.Requests;
using FileService.Business.Dtos.Responses;
using Shared.Contracts.Responses;

namespace FileService.Business.Services.Interfaces
{
    public interface IFileService
    {
        Task<PagedResult<AttachedFileResponse>> GetPagedFilesAsync(FileFilterRequest request);
        Task<AttachedFileResponse> GetFileByIdAsync(Guid id);
        Task<AttachedFileResponse> UploadFileAsync(UploadFileRequest request, Guid currentUserId);
        Task<AttachedFileResponse> UpdateMetadataAsync(Guid id, UpdateFileMetadataRequest request, Guid currentUserId);
        Task<bool> DeleteFileAsync(Guid id, Guid currentUserId);
        Task<(Stream Stream, string ContentType, string DownloadFileName)> DownloadFileAsync(Guid id);
    }
}

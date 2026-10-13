using System.Security.Claims;
using FileService.Business.Dtos.Requests;
using FileService.Business.Dtos.Responses;
using FileService.Business.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shared.Contracts.Responses;

namespace FileService.API.Controllers
{
    [ApiController]
    [Route("api/files")]
    [Authorize]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;

        public FileController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaged([FromQuery] FileFilterRequest request)
        {
            var result = await _fileService.GetPagedFilesAsync(request);
            return Ok(ApiResponse<PagedResult<AttachedFileResponse>>.SuccessResult(result));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _fileService.GetFileByIdAsync(id);
            return Ok(ApiResponse<AttachedFileResponse>.SuccessResult(result));
        }

        [HttpPost("upload")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> Upload([FromForm] UploadFileRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _fileService.UploadFileAsync(request, currentUserId);
            return Ok(ApiResponse<AttachedFileResponse>.SuccessResult(result, "Tải tệp tin lên thành công."));
        }

        [HttpGet("{id:guid}/download")]
        [AllowAnonymous] // Cho phép download file công khai hoặc theo token query
        public async Task<IActionResult> Download(Guid id)
        {
            var (stream, contentType, downloadFileName) = await _fileService.DownloadFileAsync(id);
            return File(stream, contentType, downloadFileName);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFileMetadataRequest request)
        {
            var currentUserId = GetCurrentUserId();
            var result = await _fileService.UpdateMetadataAsync(id, request, currentUserId);
            return Ok(ApiResponse<AttachedFileResponse>.SuccessResult(result, "Cập nhật thông tin tệp tin thành công."));
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var currentUserId = GetCurrentUserId();
            await _fileService.DeleteFileAsync(id, currentUserId);
            return Ok(ApiResponse<bool>.SuccessResult(true, "Xóa tệp tin thành công."));
        }

        private Guid GetCurrentUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? User.FindFirst("sub")?.Value;
            if (Guid.TryParse(userIdClaim, out var userId))
            {
                return userId;
            }
            return Guid.Empty;
        }
    }
}

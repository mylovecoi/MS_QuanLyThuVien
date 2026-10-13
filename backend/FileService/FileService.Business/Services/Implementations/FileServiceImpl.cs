using System.Linq.Expressions;
using FileService.Business.Dtos.Requests;
using FileService.Business.Dtos.Responses;
using FileService.Business.Mappers;
using FileService.Business.Services.Interfaces;
using FileService.Business.Validators;
using FileService.DataAccess.Entities;
using FileService.DataAccess.Repositories.Interfaces;
using FluentValidation;
using Shared.Contracts.Exceptions;
using Shared.Contracts.Responses;

namespace FileService.Business.Services.Implementations
{
    public class FileServiceImpl : IFileService
    {
        private readonly IAttachedFileRepository _fileRepository;
        private readonly IStorageProvider _storageProvider;
        private readonly IValidator<UploadFileRequest> _uploadValidator;
        private readonly IValidator<UpdateFileMetadataRequest> _updateValidator;

        public FileServiceImpl(
            IAttachedFileRepository fileRepository,
            IStorageProvider storageProvider,
            IValidator<UploadFileRequest> uploadValidator,
            IValidator<UpdateFileMetadataRequest> updateValidator)
        {
            _fileRepository = fileRepository;
            _storageProvider = storageProvider;
            _uploadValidator = uploadValidator;
            _updateValidator = updateValidator;
        }

        public async Task<PagedResult<AttachedFileResponse>> GetPagedFilesAsync(FileFilterRequest request)
        {
            int pageSize = Math.Clamp(request.PageSize, 1, PagedResultDefaults.MaxPageSize);
            int page = request.Page < 1 ? 1 : request.Page;

            Expression<Func<AttachedFileEntity, bool>> predicate = x => !x.IsDeleted;

            if (!string.IsNullOrWhiteSpace(request.Keyword))
            {
                var kw = request.Keyword.Trim().ToLower();
                Expression<Func<AttachedFileEntity, bool>> keywordExpr = x =>
                    x.FileName.ToLower().Contains(kw) || (x.MoTa != null && x.MoTa.ToLower().Contains(kw));

                predicate = CombinePredicates(predicate, keywordExpr);
            }

            if (request.LoaiFile.HasValue)
            {
                int loai = request.LoaiFile.Value;
                Expression<Func<AttachedFileEntity, bool>> loaiExpr = x => x.LoaiFile == loai;
                predicate = CombinePredicates(predicate, loaiExpr);
            }

            var pagedResult = await _fileRepository.GetPagedAsync(page, pageSize, predicate);

            var responses = pagedResult.ItemsOf<AttachedFileEntity>()
                .Select(x => x.ToResponse())
                .ToList();

            return PagedResult<AttachedFileResponse>.Create(
                responses,
                pagedResult.TotalCount,
                pagedResult.Page,
                pagedResult.PageSize
            );
        }

        public async Task<AttachedFileResponse> GetFileByIdAsync(Guid id)
        {
            var entity = await _fileRepository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
            {
                throw new NotFoundException($"Không tìm thấy tập tin đính kèm có ID: {id}");
            }

            return entity.ToResponse();
        }

        public async Task<AttachedFileResponse> UploadFileAsync(UploadFileRequest request, Guid currentUserId)
        {
            await ValidationRunner.EnsureValidAsync(_uploadValidator, request);

            var (fileUrl, relativePath, sha256) = await _storageProvider.SaveFileAsync(request.File);

            var entity = new AttachedFileEntity
            {
                Id = Guid.NewGuid(),
                Url = relativePath, // Lưu path tương đối để phục vụ get stream / download
                FileName = request.File.FileName,
                ContentType = string.IsNullOrWhiteSpace(request.File.ContentType) ? "application/octet-stream" : request.File.ContentType,
                MoTa = request.MoTa,
                FileSize = request.File.Length,
                LoaiFile = request.LoaiFile,
                Sha256 = sha256,
                CreatedBy = currentUserId,
                CreatedAt = DateTime.UtcNow,
                UpdatedBy = currentUserId,
                UpdatedAt = DateTime.UtcNow,
                IsDeleted = false
            };

            await _fileRepository.AddAsync(entity);
            await _fileRepository.SaveChangesAsync();

            // Cập nhật Url đầy đủ cho response client
            var response = entity.ToResponse();
            response.Url = fileUrl;
            return response;
        }

        public async Task<AttachedFileResponse> UpdateMetadataAsync(Guid id, UpdateFileMetadataRequest request, Guid currentUserId)
        {
            await ValidationRunner.EnsureValidAsync(_updateValidator, request);

            var entity = await _fileRepository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
            {
                throw new NotFoundException($"Không tìm thấy tập tin đính kèm có ID: {id}");
            }

            if (!string.IsNullOrWhiteSpace(request.FileName))
            {
                entity.FileName = request.FileName.Trim();
            }

            if (request.MoTa != null)
            {
                entity.MoTa = request.MoTa;
            }

            if (request.LoaiFile.HasValue)
            {
                entity.LoaiFile = request.LoaiFile.Value;
            }

            entity.UpdatedBy = currentUserId;
            entity.UpdatedAt = DateTime.UtcNow;

            _fileRepository.Update(entity);
            await _fileRepository.SaveChangesAsync();

            return entity.ToResponse();
        }

        public async Task<bool> DeleteFileAsync(Guid id, Guid currentUserId)
        {
            var entity = await _fileRepository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
            {
                throw new NotFoundException($"Không tìm thấy tập tin đính kèm có ID: {id}");
            }

            entity.UpdatedBy = currentUserId;
            entity.UpdatedAt = DateTime.UtcNow;

            _fileRepository.Delete(entity);
            var success = await _fileRepository.SaveChangesAsync();

            if (success)
            {
                await _storageProvider.DeleteFileAsync(entity.Url);
            }

            return success;
        }

        public async Task<(Stream Stream, string ContentType, string DownloadFileName)> DownloadFileAsync(Guid id)
        {
            var entity = await _fileRepository.GetByIdAsync(id);
            if (entity == null || entity.IsDeleted)
            {
                throw new NotFoundException($"Không tìm thấy tập tin đính kèm có ID: {id}");
            }

            return await _storageProvider.GetFileStreamAsync(entity.Url, entity.FileName, entity.ContentType);
        }

        private static Expression<Func<T, bool>> CombinePredicates<T>(
            Expression<Func<T, bool>> expr1, Expression<Func<T, bool>> expr2)
        {
            var parameter = Expression.Parameter(typeof(T));
            var leftVisitor = new ReplaceExpressionVisitor(expr1.Parameters[0], parameter);
            var left = leftVisitor.Visit(expr1.Body);
            var rightVisitor = new ReplaceExpressionVisitor(expr2.Parameters[0], parameter);
            var right = rightVisitor.Visit(expr2.Body);

            return Expression.Lambda<Func<T, bool>>(Expression.AndAlso(left!, right!), parameter);
        }

        private class ReplaceExpressionVisitor : ExpressionVisitor
        {
            private readonly Expression _oldValue;
            private readonly Expression _newValue;

            public ReplaceExpressionVisitor(Expression oldValue, Expression newValue)
            {
                _oldValue = oldValue;
                _newValue = newValue;
            }

            public override Expression? Visit(Expression? node)
            {
                if (node == _oldValue) return _newValue;
                return base.Visit(node);
            }
        }
    }
}

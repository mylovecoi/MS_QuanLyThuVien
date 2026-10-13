using FileService.DataAccess.Entities;

namespace FileService.DataAccess.Repositories.Interfaces
{
    public interface IAttachedFileRepository : IRepository<AttachedFileEntity>
    {
        Task<AttachedFileEntity?> GetBySha256Async(string sha256);
        Task<IEnumerable<AttachedFileEntity>> GetByCreatedByAsync(Guid userId);
    }
}

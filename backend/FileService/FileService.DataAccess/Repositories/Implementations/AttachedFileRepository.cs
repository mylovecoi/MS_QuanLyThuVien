using FileService.DataAccess.Contexts;
using FileService.DataAccess.Entities;
using FileService.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FileService.DataAccess.Repositories.Implementations
{
    public class AttachedFileRepository : Repository<AttachedFileEntity>, IAttachedFileRepository
    {
        public AttachedFileRepository(FileDbContext context) : base(context)
        {
        }

        public async Task<AttachedFileEntity?> GetBySha256Async(string sha256)
        {
            if (string.IsNullOrWhiteSpace(sha256)) return null;
            return await _dbSet.FirstOrDefaultAsync(x => x.Sha256 == sha256);
        }

        public async Task<IEnumerable<AttachedFileEntity>> GetByCreatedByAsync(Guid userId)
        {
            return await _dbSet.Where(x => x.CreatedBy == userId).AsNoTracking().ToListAsync();
        }
    }
}

using IdentityService.DataAccess.Contexts;
using IdentityService.DataAccess.Entities;
using IdentityService.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DataAccess.Repositories.Implementations
{
    public class RoleRepository : Repository<RoleEntity>, IRoleRepository
    {
        public RoleRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<RoleEntity?> GetByCodeAsync(string code)
        {
            return await _dbSet.FirstOrDefaultAsync(r => r.Code == code && !r.IsDeleted);
        }

        public async Task<RoleEntity?> GetWithPermissionsAsync(Guid roleId)
        {
            return await _dbSet
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted);
        }

        public async Task<IEnumerable<RoleEntity>> GetAllWithPermissionsAsync()
        {
            return await _dbSet
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Where(r => !r.IsDeleted)
                .ToListAsync();
        }
    }
}

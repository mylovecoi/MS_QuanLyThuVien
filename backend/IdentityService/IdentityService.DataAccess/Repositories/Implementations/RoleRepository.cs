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
            return await RolesWithPermissions()
                .FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted);
        }

        public async Task<RoleEntity?> GetWithAllRelationsAsync(Guid roleId)
        {
            return await _dbSet
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission)
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.Id == roleId && !r.IsDeleted);
        }

        public async Task<IEnumerable<RoleEntity>> GetAllWithPermissionsAsync()
        {
            return await RolesWithPermissions()
                .Where(r => !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IEnumerable<RoleEntity>> GetByIdsAsync(IEnumerable<Guid> roleIds)
        {
            var ids = roleIds.Where(id => id != Guid.Empty).Distinct().ToList();
            if (ids.Count == 0)
            {
                return new List<RoleEntity>();
            }

            return await _dbSet
                .Where(r => ids.Contains(r.Id) && !r.IsDeleted)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<PermissionEntity>> GetAllPermissionsAsync()
        {
            return await _context.Permissions
                .Where(p => !p.IsDeleted)
                .OrderBy(p => p.Module)
                .ThenBy(p => p.Code)
                .ToListAsync();
        }

        public async Task<int> CountUserRolesAsync(Guid roleId)
        {
            return await _context.UserRoles.CountAsync(ur => ur.RoleId == roleId);
        }

        /// <summary>
        /// Paging ở tầng DB nhưng vẫn phải nạp RolePermissions cho đúng 1 trang,
        /// nếu không thì RoleMapper sẽ ra danh sách permission rỗng.
        /// </summary>
        public async Task<PagedResult> GetPagedWithPermissionsAsync(int page, int pageSize)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 200) pageSize = 200;

            var query = RolesWithPermissions().Where(r => !r.IsDeleted);

            var totalCount = await query.CountAsync();
            var totalPages = totalCount <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .OrderBy(r => r.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult
            {
                Items = items.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                TotalPages = totalPages,
                HasPrevious = page > 1,
                HasNext = page < totalPages
            };
        }

        public async Task ReplacePermissionsAsync(Guid roleId, IReadOnlyCollection<Guid> permissionIds)
        {
            // Chỉ thêm/xóa phần chênh lệch. Nếu xóa hết rồi add lại thì các bản ghi
            // cũ vẫn đang được track ở state Deleted, khiến EF ném lỗi
            // "another instance with the same key is already being tracked"
            // cho mọi quyền hạn client giữ nguyên khi sửa role.
            var desired = permissionIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToHashSet();

            var existing = await _context.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .ToListAsync();

            var current = existing.Select(rp => rp.PermissionId).ToHashSet();

            var toRemove = existing
                .Where(rp => !desired.Contains(rp.PermissionId))
                .ToList();

            var toAdd = desired
                .Where(permissionId => !current.Contains(permissionId))
                .Select(permissionId => new RolePermissionEntity
                {
                    RoleId = roleId,
                    PermissionId = permissionId
                })
                .ToList();

            if (toRemove.Count > 0)
            {
                _context.RolePermissions.RemoveRange(toRemove);
            }

            if (toAdd.Count > 0)
            {
                await _context.RolePermissions.AddRangeAsync(toAdd);
            }
        }

        public async Task<bool> HardDeleteAsync(Guid roleId)
        {
            var role = await _dbSet
                .Include(r => r.RolePermissions)
                .Include(r => r.UserRoles)
                .FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
            {
                return false;
            }

            // Xóa cứng cả bản ghi liên kết, nếu không sẽ tồn dư role_id trỏ tới
            // role không còn tồn tại (ràng buộc FK cascade không áp dụng cho xóa cứng).
            if (role.RolePermissions.Count > 0)
            {
                _context.RolePermissions.RemoveRange(role.RolePermissions);
            }

            if (role.UserRoles.Count > 0)
            {
                _context.UserRoles.RemoveRange(role.UserRoles);
            }

            _dbSet.Remove(role);
            return true;
        }

        private IQueryable<RoleEntity> RolesWithPermissions()
        {
            return _dbSet
                .Include(r => r.RolePermissions)
                    .ThenInclude(rp => rp.Permission);
        }
    }
}
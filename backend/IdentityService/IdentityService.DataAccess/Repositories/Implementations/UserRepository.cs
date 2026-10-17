using IdentityService.DataAccess.Contexts;
using IdentityService.DataAccess.Entities;
using IdentityService.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DataAccess.Repositories.Implementations
{
    public class UserRepository : Repository<UserEntity>, IUserRepository
    {
        public UserRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<UserEntity?> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task<UserEntity?> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email && !u.IsDeleted);
        }

        public async Task<UserEntity?> GetWithRolesAndPermissionsByIdAsync(Guid userId)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }

        public async Task<UserEntity?> GetWithRolesAndPermissionsByUsernameAsync(string username)
        {
            return await _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role)
                        .ThenInclude(r => r.RolePermissions)
                            .ThenInclude(rp => rp.Permission)
                .FirstOrDefaultAsync(u => u.Username == username && !u.IsDeleted);
        }

        public async Task<UserEntity?> GetWithRolesByIdAsync(Guid userId)
        {
            return await UsersWithRoles()
                .FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted);
        }

        public async Task<PagedResult> GetPagedWithRolesAsync(int page, int pageSize, string? search)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 200) pageSize = 200;

            var query = UsersWithRoles().Where(u => !u.IsDeleted);

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(u =>
                    u.Username.Contains(term) ||
                    u.Email.Contains(term) ||
                    u.FullName.Contains(term));
            }

            var totalCount = await query.CountAsync();
            var totalPages = totalCount <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
                .OrderByDescending(u => u.CreatedAt)
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

        public async Task ReplaceRolesAsync(Guid userId, IReadOnlyCollection<Guid> roleIds)
        {
            var desired = roleIds
                .Where(id => id != Guid.Empty)
                .Distinct()
                .ToHashSet();

            var existing = await _context.UserRoles
                .Where(ur => ur.UserId == userId)
                .ToListAsync();

            var current = existing.Select(ur => ur.RoleId).ToHashSet();

            var toRemove = existing
                .Where(ur => !desired.Contains(ur.RoleId))
                .ToList();

            var toAdd = desired
                .Where(roleId => !current.Contains(roleId))
                .Select(roleId => new UserRoleEntity
                {
                    UserId = userId,
                    RoleId = roleId
                })
                .ToList();

            if (toRemove.Count > 0)
            {
                _context.UserRoles.RemoveRange(toRemove);
            }

            if (toAdd.Count > 0)
            {
                await _context.UserRoles.AddRangeAsync(toAdd);
            }
        }

        private IQueryable<UserEntity> UsersWithRoles()
        {
            return _dbSet
                .Include(u => u.UserRoles)
                    .ThenInclude(ur => ur.Role);
        }
    }
}

using System.Linq.Expressions;
using FileService.DataAccess.Contexts;
using FileService.DataAccess.Entities;
using FileService.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FileService.DataAccess.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly FileDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(FileDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.AsNoTracking().ToListAsync();
        }

        public async Task<PagedResult> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? predicate = null)
        {
            IQueryable<T> query = _dbSet.AsNoTracking();

            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync();
            var effectivePageSize = pageSize < 1 ? 20 : pageSize;
            var effectivePage = page < 1 ? 1 : page;

            var items = await query
                .OrderByDescending(x => x.CreatedAt)
                .Skip((effectivePage - 1) * effectivePageSize)
                .Take(effectivePageSize)
                .ToListAsync();

            var totalPages = totalCount <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)effectivePageSize);

            return new PagedResult
            {
                Items = items.Cast<object>().ToList(),
                TotalCount = totalCount,
                Page = effectivePage,
                PageSize = effectivePageSize,
                TotalPages = totalPages,
                HasPrevious = effectivePage > 1,
                HasNext = effectivePage < totalPages
            };
        }

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(predicate).AsNoTracking().ToListAsync();
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            _dbSet.Update(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }
}

using Microsoft.EntityFrameworkCore;
using ProfileService.DataAccess.Contexts;
using ProfileService.DataAccess.Entities;
using ProfileService.DataAccess.Repositories.Interfaces;
using System.Linq.Expressions;

namespace ProfileService.DataAccess.Repositories.Implementations
{
    public class Repository<T> : IRepository<T> where T : BaseEntity
    {
        protected readonly ProfileDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public Repository(ProfileDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
        }

        public async Task<PagedResult> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? predicate = null)
        {
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 200) pageSize = 200;

            var query = _dbSet.Where(e => !e.IsDeleted);
            if (predicate != null)
            {
                query = query.Where(predicate);
            }

            var totalCount = await query.CountAsync();
            var totalPages = totalCount <= 0 ? 0 : (int)Math.Ceiling(totalCount / (double)pageSize);

            var items = await query
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

        public async Task<T?> GetByIdAsync(Guid id)
        {
            return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<T?> FindAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(e => !e.IsDeleted).FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate)
        {
            return await _dbSet.Where(e => !e.IsDeleted).Where(predicate).ToListAsync();
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
            // UpdatedAt và UpdatedBy sẽ được DbContext tự điền khi SaveChangesAsync()
            _dbSet.Update(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }

    public class ProfileRepository : Repository<ProfileEntity>, IProfileRepository
    {
        public ProfileRepository(ProfileDbContext context) : base(context)
        {
        }

        public async Task<ProfileEntity?> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
        }

        public async Task<ProfileEntity?> GetByMaCanBoAsync(string maCanBo)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.MaCanBo == maCanBo && !p.IsDeleted);
        }

        public async Task<IEnumerable<ProfileEntity>> GetByPhongBanIdAsync(Guid phongBanId)
        {
            return await _dbSet.Where(p => p.PhongBanId == phongBanId && !p.IsDeleted).ToListAsync();
        }

        /// <summary>
        /// Lấy hồ sơ theo UserId, tự tạo nếu chưa có.
        ///
        /// Ba nhánh xử lý, xếp theo tần suất gặp:
        ///  1. Đã có hồ sơ -> trả về luôn (nhánh phổ biến nhất, không ghi DB).
        ///  2. Có bản ghi đã xoá mềm -> hồi sinh bản ghi cũ. Bắt buộc phải hồi
        ///     sinh chứ không insert mới, vì HasIndex(UserId).IsUnique() không
        ///     lọc IsDeleted nên bản ghi đã xoá vẫn giữ chỗ trên UserId và
        ///     insert mới chắc chắn dính lỗi unique.
        ///  3. Chưa có gì -> insert. Nếu hai request cùng tạo thì unique index
        ///     giữ dữ liệu nhất quán; request thua cuộc đọc lại bản ghi đã tạo.
        /// </summary>
        public async Task<(ProfileEntity Profile, bool Created)> EnsureExistsAsync(
            Guid userId,
            string? fullName = null,
            string? workEmail = null)
        {
            var existing = await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
            if (existing != null)
            {
                return (existing, false);
            }

            var softDeleted = await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId);
            if (softDeleted != null)
            {
                softDeleted.IsDeleted = false;
                await _context.SaveChangesAsync();
                return (softDeleted, true);
            }

            var entity = new ProfileEntity
            {
                UserId = userId,
                FullName = fullName ?? string.Empty,
                WorkEmail = workEmail ?? string.Empty
            };

            await _dbSet.AddAsync(entity);

            try
            {
                await _context.SaveChangesAsync();
                return (entity, true);
            }
            catch (DbUpdateException)
            {
                // Bản ghi thuộc request này đã hỏng sau khi insert thất bại,
                // phải detach ra để không làm hỏng các thao tác sau trong cùng context.
                _context.Entry(entity).State = EntityState.Detached;

                var winner = await _dbSet
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);

                if (winner == null)
                {
                    throw;
                }

                return (winner, false);
            }
        }
    }

    public class PhongBanRepository : Repository<PhongBanEntity>, IPhongBanRepository
    {
        public PhongBanRepository(ProfileDbContext context) : base(context)
        {
        }

        public async Task<PhongBanEntity?> GetByMaPhongBanAsync(string maPhongBan)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.MaPhongBan == maPhongBan && !p.IsDeleted);
        }

        public async Task<IEnumerable<PhongBanEntity>> GetChildrenAsync(Guid parentId)
        {
            return await _dbSet.Where(p => p.ParentId == parentId && !p.IsDeleted).OrderBy(p => p.OrderNo).ToListAsync();
        }
    }

    public class ChucVuRepository : Repository<ChucVuEntity>, IChucVuRepository
    {
        public ChucVuRepository(ProfileDbContext context) : base(context)
        {
        }

        public async Task<ChucVuEntity?> GetByMaChucVuAsync(string maChucVu)
        {
            return await _dbSet.FirstOrDefaultAsync(c => c.MaChucVu == maChucVu && !c.IsDeleted);
        }
    }
}

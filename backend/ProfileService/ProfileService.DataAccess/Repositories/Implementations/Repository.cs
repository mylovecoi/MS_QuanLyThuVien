using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ProfileService.DataAccess.Contexts;
using ProfileService.DataAccess.Entities;
using ProfileService.DataAccess.Repositories.Interfaces;

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
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public void Delete(T entity)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
            _dbSet.Update(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync() > 0;
        }
    }

    public class UserProfileRepository : Repository<UserProfileEntity>, IUserProfileRepository
    {
        public UserProfileRepository(ProfileDbContext context) : base(context)
        {
        }

        public async Task<UserProfileEntity?> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.UserId == userId && !p.IsDeleted);
        }

        public async Task<UserProfileEntity?> GetByMaCanBoAsync(string maCanBo)
        {
            return await _dbSet.FirstOrDefaultAsync(p => p.MaCanBo == maCanBo && !p.IsDeleted);
        }

        public async Task<IEnumerable<UserProfileEntity>> GetByPhongBanIdAsync(Guid phongBanId)
        {
            return await _dbSet.Where(p => p.PhongBanId == phongBanId && !p.IsDeleted).ToListAsync();
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

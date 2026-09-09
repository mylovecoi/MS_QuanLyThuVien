using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using ProfileService.DataAccess.Entities;

namespace ProfileService.DataAccess.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task<IEnumerable<T>> FindAllAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> SaveChangesAsync();
    }

    public interface IUserProfileRepository : IRepository<UserProfileEntity>
    {
        Task<UserProfileEntity?> GetByUserIdAsync(Guid userId);
        Task<UserProfileEntity?> GetByMaCanBoAsync(string maCanBo);
        Task<IEnumerable<UserProfileEntity>> GetByPhongBanIdAsync(Guid phongBanId);
    }

    public interface IPhongBanRepository : IRepository<PhongBanEntity>
    {
        Task<PhongBanEntity?> GetByMaPhongBanAsync(string maPhongBan);
        Task<IEnumerable<PhongBanEntity>> GetChildrenAsync(Guid parentId);
    }

    public interface IChucVuRepository : IRepository<ChucVuEntity>
    {
        Task<ChucVuEntity?> GetByMaChucVuAsync(string maChucVu);
    }
}

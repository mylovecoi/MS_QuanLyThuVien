using ProfileService.DataAccess.Entities;
using System.Linq.Expressions;

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

    public interface IProfileRepository : IRepository<ProfileEntity>
    {
        Task<ProfileEntity?> GetByUserIdAsync(Guid userId);
        Task<ProfileEntity?> GetByMaCanBoAsync(string maCanBo);
        Task<IEnumerable<ProfileEntity>> GetByPhongBanIdAsync(Guid phongBanId);
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

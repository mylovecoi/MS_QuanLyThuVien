using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using ProfileService.DataAccess.Entities;

namespace ProfileService.DataAccess.Repositories.Interfaces
{
    /// <summary>
    /// Kết quả phân trang ở tầng DataAccess. Items giữ dạng object vì repository
    /// là generic; tầng Business sẽ cast về entity rồi map sang DTO.
    /// </summary>
    public class PagedResult
    {
        public List<object> Items { get; set; } = new List<object>();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPrevious { get; set; }
        public bool HasNext { get; set; }

        public IEnumerable<T> ItemsOf<T>() where T : class => Items.Cast<T>();
    }

    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<PagedResult> GetPagedAsync(int page, int pageSize, Expression<Func<T, bool>>? predicate = null);
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

        /// <summary>
        /// Lấy hồ sơ theo UserId, tự tạo nếu chưa có.
        /// Trả về kèm cờ Created để tầng trên ghi log khi hồ sơ vừa được tạo.
        /// </summary>
        Task<(ProfileEntity Profile, bool Created)> EnsureExistsAsync(
            Guid userId,
            string? fullName = null,
            string? workEmail = null);
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

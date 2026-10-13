using System.Collections.Generic;
using System.Linq.Expressions;
using FileService.DataAccess.Entities;

namespace FileService.DataAccess.Repositories.Interfaces
{
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
}

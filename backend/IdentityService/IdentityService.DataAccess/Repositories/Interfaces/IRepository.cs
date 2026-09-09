using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using IdentityService.DataAccess.Entities;

namespace IdentityService.DataAccess.Repositories.Interfaces
{
    public interface IRepository<T> where T : BaseEntity
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T?> GetByIdAsync(Guid id);
        Task<T?> FindAsync(Expression<Func<T, bool>> predicate);
        Task AddAsync(T entity);
        void Update(T entity);
        void Delete(T entity);
        Task<bool> SaveChangesAsync();
    }

    public interface IUserRepository : IRepository<UserEntity>
    {
        Task<UserEntity?> GetByUsernameAsync(string username);
        Task<UserEntity?> GetByEmailAsync(string email);
    }
}

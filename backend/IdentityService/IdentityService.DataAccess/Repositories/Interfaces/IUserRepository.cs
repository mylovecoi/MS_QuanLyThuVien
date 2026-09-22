using IdentityService.DataAccess.Entities;

namespace IdentityService.DataAccess.Repositories.Interfaces
{
    public interface IUserRepository : IRepository<UserEntity>
    {
        Task<UserEntity?> GetByUsernameAsync(string username);
        Task<UserEntity?> GetByEmailAsync(string email);
        Task<UserEntity?> GetWithRolesAndPermissionsByIdAsync(Guid userId);
        Task<UserEntity?> GetWithRolesAndPermissionsByUsernameAsync(string username);
    }
}

using IdentityService.DataAccess.Entities;

namespace IdentityService.DataAccess.Repositories.Interfaces
{
    public interface IRoleRepository : IRepository<RoleEntity>
    {
        Task<RoleEntity?> GetByCodeAsync(string code);
        Task<RoleEntity?> GetWithPermissionsAsync(Guid roleId);
        Task<IEnumerable<RoleEntity>> GetAllWithPermissionsAsync();
    }
}

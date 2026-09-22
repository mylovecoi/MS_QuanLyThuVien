using IdentityService.DataAccess.Entities;

namespace IdentityService.DataAccess.Repositories.Interfaces
{
    public interface IInvalidatedTokenRepository : IRepository<InvalidatedTokenEntity>
    {
        Task<bool> IsInvalidatedAsync(string jti);
    }
}

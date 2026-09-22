using IdentityService.DataAccess.Entities;

namespace IdentityService.DataAccess.Repositories.Interfaces
{
    public interface IRefreshTokenRepository : IRepository<RefreshTokenEntity>
    {
        Task<RefreshTokenEntity?> GetByTokenAsync(string token);
        Task RevokeUserTokensAsync(Guid userId);
    }
}

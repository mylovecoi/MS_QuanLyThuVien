using IdentityService.DataAccess.Contexts;
using IdentityService.DataAccess.Entities;
using IdentityService.DataAccess.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace IdentityService.DataAccess.Repositories.Implementations
{
    public class InvalidatedTokenRepository : Repository<InvalidatedTokenEntity>, IInvalidatedTokenRepository
    {
        public InvalidatedTokenRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<bool> IsInvalidatedAsync(string jti)
        {
            return await _dbSet.AnyAsync(it => it.Jti == jti);
        }
    }
}

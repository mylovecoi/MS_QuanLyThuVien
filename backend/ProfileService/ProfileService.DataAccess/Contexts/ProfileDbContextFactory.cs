using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace ProfileService.DataAccess.Contexts
{
    public class ProfileDbContextFactory : IDesignTimeDbContextFactory<ProfileDbContext>
    {
        public ProfileDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProfileDbContext>();
            optionsBuilder.UseSqlServer("Server=DESKTOP-0LER7OP\\SQLEXPRESS;Database=ProfileDB;Trusted_Connection=True;TrustServerCertificate=True");

            return new ProfileDbContext(optionsBuilder.Options);
        }
    }
}

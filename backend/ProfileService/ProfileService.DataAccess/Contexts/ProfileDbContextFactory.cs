using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ProfileService.DataAccess.Contexts
{
    public class ProfileDbContextFactory : IDesignTimeDbContextFactory<ProfileDbContext>
    {
        public ProfileDbContext CreateDbContext(string[] args)
        {
            var basePath = Path.Combine(Directory.GetCurrentDirectory(), "../ProfileService.API");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            var optionsBuilder = new DbContextOptionsBuilder<ProfileDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ProfileDbContext(optionsBuilder.Options);
        }
    }
}

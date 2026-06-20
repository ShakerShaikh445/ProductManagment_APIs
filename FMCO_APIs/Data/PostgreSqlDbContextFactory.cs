using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ProductManagment_APIs.Data
{
    public class PostgreSqlDbContextFactory
        : IDesignTimeDbContextFactory<PostgreSqlDbContext>
    {
        public PostgreSqlDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder =
                new DbContextOptionsBuilder<PostgreSqlDbContext>();

            optionsBuilder.UseNpgsql(
                configuration.GetConnectionString("PostgreSql"));

            return new PostgreSqlDbContext(optionsBuilder.Options);
        }
    }
}
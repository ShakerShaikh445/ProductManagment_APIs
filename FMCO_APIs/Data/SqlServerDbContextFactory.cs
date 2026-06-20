using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ProductManagment_APIs.Interface;

namespace ProductManagment_APIs.Data
{
    public class SqlServerDbContextFactory
        : IDesignTimeDbContextFactory<SqlServerDbContext>
    {
        public SqlServerDbContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var optionsBuilder =
                new DbContextOptionsBuilder<SqlServerDbContext>();

            optionsBuilder.UseSqlServer(
                configuration.GetConnectionString("SqlServer"));

            return new SqlServerDbContext(optionsBuilder.Options);
        }
    }
}
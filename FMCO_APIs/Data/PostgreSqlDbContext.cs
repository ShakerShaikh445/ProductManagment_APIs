using Microsoft.EntityFrameworkCore;

namespace ProductManagment_APIs.Data
{
    public class PostgreSqlDbContext : AppDbContext
    {
        public PostgreSqlDbContext(DbContextOptions<PostgreSqlDbContext> options)
            : base(options)
        {
        }
    }
}
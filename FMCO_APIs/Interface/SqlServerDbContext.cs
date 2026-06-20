using Microsoft.EntityFrameworkCore;
using ProductManagment_APIs.Data;

namespace ProductManagment_APIs.Interface
{
    public class SqlServerDbContext : AppDbContext
    {
        public SqlServerDbContext(DbContextOptions<SqlServerDbContext> options)
            : base(options)
        {
        }
    }
}

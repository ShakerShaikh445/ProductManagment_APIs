using ProductManagment_APIs.Model;
using ProductManagment_APIs.Service;
using Microsoft.EntityFrameworkCore;

namespace ProductManagment_APIs.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }
        public DbSet<Role> Roles { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<AppUser> MasterUsers { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Location> Locations { get; set; }

        public DbSet<Item> Items { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UserRole>()
                .HasKey(ur => new { ur.Id, ur.RoleId });

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.UserRoles)
                .HasForeignKey(ur => ur.Id);

            modelBuilder.Entity<UserRole>()
                .HasOne(ur => ur.Role)
                .WithMany(r => r.UserRoles)
                .HasForeignKey(ur => ur.RoleId);

            modelBuilder.Entity<Role>().HasData(
                new Role
                {
                    RoleId = 1,
                    RoleName = "Admin"
                },
                new Role
                {
                    RoleId = 2,
                    RoleName = "Employee"
                }
              );

            var adminUser = new AppUser
            {
                Id = 1,
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                PasswordHash = PasswordHelper.HashPassword("Admin@123"),
                IsActive = true
            };
            var Employee = new AppUser
            {
                Id = 2,
                UserName = "employee@gmail.com",
                Email = "employee@gmail.com",
                PasswordHash = PasswordHelper.HashPassword("Employee@123"),
                IsActive = true
            };
            modelBuilder.Entity<AppUser>().HasData(adminUser, Employee);

            modelBuilder.Entity<UserRole>().HasData(
                new UserRole
                {
                    Id = 1,
                    RoleId = 1
                },
                new UserRole
                {
                    Id = 2,
                    RoleId = 2
                }
            );
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            foreach (var entry in ChangeTracker.Entries<Audit>())
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAT = DateTime.UtcNow;

                    entry.Entity.CreatedBy = 1;
                }

                if (entry.State == EntityState.Modified)
                {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;

                    entry.Entity.UpdatedBy = 1;

                    entry.Property(x => x.CreatedAT).IsModified = false;
                    entry.Property(x => x.CreatedBy).IsModified = false;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}

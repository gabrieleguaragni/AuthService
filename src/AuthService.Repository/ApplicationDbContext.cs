using AuthService.Repository.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repository
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<UsersTable>().ToTable("Users");
            modelBuilder.Entity<UsersTable>().HasKey(x => x.IDUser);
            modelBuilder.Entity<UsersTable>().Property(e => e.IDUser).ValueGeneratedOnAdd();


            modelBuilder.Entity<RolesTable>().ToTable("Roles");
            modelBuilder.Entity<RolesTable>().HasKey(x => x.IDRole);
            modelBuilder.Entity<RolesTable>().Property(e => e.IDRole).ValueGeneratedOnAdd();


            modelBuilder.Entity<UserRolesTable>().ToTable("UserRoles");
            modelBuilder.Entity<UserRolesTable>().HasKey(x => x.IDUserRole);
            modelBuilder.Entity<UserRolesTable>().Property(e => e.IDUserRole).ValueGeneratedOnAdd();


            modelBuilder.Entity<UsersTable>()
                .HasMany(x => x.UserRoles)
                .WithOne(x => x.User)
                .HasForeignKey(x => new {x.IDUser});

            modelBuilder.Entity<RolesTable>()
               .HasMany(x => x.UserRoles)
               .WithOne(x => x.Role)
               .HasForeignKey(x => new { x.IDRole });
        }

        public DbSet<UsersTable> Users { get; set; }
        public DbSet<RolesTable> Roles { get; set; }
        public DbSet<UserRolesTable> UserRoles { get; set; }
    }
}

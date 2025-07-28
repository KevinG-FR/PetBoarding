using Microsoft.EntityFrameworkCore;

using PetBoarding_Domain.Users;

namespace PetBoarding_Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Permission> Users { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);

            modelBuilder.HasDefaultSchema("PetBoarding");
        }
    }
}
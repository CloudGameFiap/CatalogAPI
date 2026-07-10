using CloudGameCatalog.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CloudGameCatalog.Infrastructure.EntityFramework
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            modelBuilder.Entity<Game>();
            modelBuilder.Entity<UserGame>();
        }
    }
}

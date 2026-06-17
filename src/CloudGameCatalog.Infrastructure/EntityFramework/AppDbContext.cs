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
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Game>();
        }
    }
}

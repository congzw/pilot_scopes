using Microsoft.EntityFrameworkCore;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;

// ReSharper disable once CheckNamespace
namespace SmartClass.DAL
{
    public class HblTempDbContext : DbContext
    {
        public HblTempDbContext(DbContextOptions<HblTempDbContext> options) : base(options)
        {

        }

        public DbSet<MyConnection> MyConnections { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<MyConnection>().Property(x => x.Bags).HasBagsConversion();
            modelBuilder.Entity<MyConnection>().Property(x => x.Groups).HasListConversion();
        }
    }
}

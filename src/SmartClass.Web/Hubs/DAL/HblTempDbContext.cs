using Microsoft.EntityFrameworkCore;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientConnections;
using SmartClass.Common.ScopeHubs.ClientMonitors.ClientGroups;

// ReSharper disable once CheckNamespace
namespace SmartClass.DAL
{
    public class HblTempDbContext : DbContext
    {
        public HblTempDbContext(DbContextOptions<HblTempDbContext> options) : base(options)
        {
        }

        public DbSet<MyConnection> MyConnections { get; set; }
        public DbSet<ScopeClientGroup> ScopeClientGroups { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<MyConnection>().Property(x => x.Bags).HasBagsConversion();
            //builder.Entity<MyConnection>().Property(x => x.Groups).HasListConversion();
            
            builder.Entity<ScopeClientGroup>(entity =>
            {
                entity.HasKey(x => new { x.ScopeId, x.ClientId, x.Group }).HasName("PK_ScopeClientGroup");
            });
        }
    }
}

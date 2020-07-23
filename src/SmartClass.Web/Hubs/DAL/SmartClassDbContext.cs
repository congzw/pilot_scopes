using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using SmartClass.Common.Scopes;

// ReSharper disable once CheckNamespace
namespace SmartClass.DAL
{
    public class SmartClassDbContext : DbContext
    {
        public SmartClassDbContext(DbContextOptions options) : base(options)
        {
        }

        public override int SaveChanges()
        {
            ApplyExtensionConcepts();
            return base.SaveChanges();
        }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            ApplyExtensionConcepts();
            return base.SaveChangesAsync(cancellationToken);
        }

        protected virtual void ApplyExtensionConcepts()
        {
            var userId = GetAuditUserId();
            foreach (var entry in ChangeTracker.Entries().ToList())
            {
                if (entry.State != EntityState.Modified && CheckOwnedEntityChange(entry))
                {
                    Entry(entry.Entity).State = EntityState.Modified;
                }

                ApplyConcepts(entry, userId);
            }
        }
        public static bool CheckOwnedEntityChange(EntityEntry entry)
        {
            return entry.State == EntityState.Modified ||
                   entry.References.Any(r =>
                       r.TargetEntry != null && r.TargetEntry.Metadata.IsOwned() && CheckOwnedEntityChange(r.TargetEntry));
        }
        protected virtual long? GetAuditUserId()
        {
            return null;
        }
        protected virtual void ApplyConcepts(EntityEntry entry, long? userId)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    ApplyConceptsForAddedEntity(entry, userId);
                    break;
                case EntityState.Modified:
                    ApplyConceptsForModifiedEntity(entry, userId);
                    break;
                case EntityState.Deleted:
                    ApplyConceptsForDeletedEntity(entry, userId);
                    break;
            }
        }
        protected virtual void ApplyConceptsForAddedEntity(EntityEntry entry, long? userId)
        {
            CheckAndSetProperties(entry);
        }

        protected virtual void ApplyConceptsForModifiedEntity(EntityEntry entry, long? userId)
        {
        }

        protected virtual void ApplyConceptsForDeletedEntity(EntityEntry entry, long? userId)
        {
        }

        protected virtual void CheckAndSetProperties(EntityEntry entry)
        {
            SetScopeId(entry);
        }

        private void SetScopeId(EntityEntry entityEntry)
        {
            if (entityEntry.Entity is IHaveScopeId haveScope)
            {
                if (string.IsNullOrWhiteSpace(haveScope.ScopeId))
                {
                    var scopeId = ScopeContext.GetScopeId();
                    haveScope.ScopeId = scopeId;
                }
            }
        }
    }
}

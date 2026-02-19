using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Persistance
{
    public abstract class AuditableDbContext : DbContext
    {
        public AuditableDbContext(DbContextOptions options) : base(options)
        {
        }
        public virtual async Task<int> SaveChangesAsync(string? username = "SYSTEM")
        {
            foreach (var entry in base.ChangeTracker.Entries()
                         .Where(q => q.State == EntityState.Added || q.State == EntityState.Modified))
            {
                if (entry.Entity is IAuditableBaseEntity auditableEntity)
                {
                    auditableEntity.LastModifiedDate = DateTime.Now;
                    auditableEntity.LastModifiedBy = username;

                    if (entry.State == EntityState.Added)
                    {
                        auditableEntity.DateCreated = DateTime.Now;
                        auditableEntity.CreatedBy = username;
                    }
                }
            }

            return await base.SaveChangesAsync();
        }

        // Must override the base method for consistency
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return SaveChangesAsync(username: "SYSTEM");
        }
    }
}

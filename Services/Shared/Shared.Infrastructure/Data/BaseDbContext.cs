using Microsoft.EntityFrameworkCore;
using Shared.Core.Entities;

namespace Shared.Infrastructure.Data
{
    public abstract class BaseDbContext : DbContext
    {
        public BaseDbContext(DbContextOptions options) : base(options)
        {
        }
        #region Save change modify
        public override int SaveChanges()
        {
            ChangeTrackerModify();
            return base.SaveChanges();
        }
        public override int SaveChanges(bool acceptAllChangesOnSuccess)
        {
            ChangeTrackerModify();
            return base.SaveChanges(acceptAllChangesOnSuccess);
        }
        public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            ChangeTrackerModify();
            return await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            ChangeTrackerModify();
            return await base.SaveChangesAsync(cancellationToken);
        }
        private void ChangeTrackerModify()
        {
            foreach (var entry in ChangeTracker.Entries<BaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = DateTime.UtcNow;
                        entry.Entity.ModifiedCount ??= 0;
                        entry.Entity.ModifiedCount++;
                        break;
                    case EntityState.Modified:
                        entry.Entity.Modified = DateTime.UtcNow;
                        entry.Entity.ModifiedCount ??= 0;
                        entry.Entity.ModifiedCount++;
                        break;
                    case EntityState.Deleted:
                        if (!entry.Entity.IsHardDeleted)
                        { 
                            entry.Entity.IsDeleted = true;
                            entry.Entity.Modified = DateTime.UtcNow;
                            entry.Entity.ModifiedCount ??= 0;
                            entry.Entity.ModifiedCount++;
                            entry.State = EntityState.Modified;
                        }
                        break;
                    //case EntityState.Unchanged:
                    //    break;
                }
            }
        }
        #endregion

    }
}

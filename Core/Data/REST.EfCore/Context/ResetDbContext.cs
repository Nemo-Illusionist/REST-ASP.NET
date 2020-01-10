using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EfCore.Context
{
    public abstract class ResetDbContext : DbContext
    {
        protected ResetDbContext(DbContextOptions options) : base(options)
        {
        }

        public void Reset()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.State != EntityState.Unchanged)
                .ToArray();

            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Modified:
                        entry.State = EntityState.Unchanged;
                        break;
                    case EntityState.Added:
                        entry.State = EntityState.Detached;
                        break;
                    case EntityState.Deleted:
                        entry.Reload();
                        break;
                }
            }
        }
    }
}
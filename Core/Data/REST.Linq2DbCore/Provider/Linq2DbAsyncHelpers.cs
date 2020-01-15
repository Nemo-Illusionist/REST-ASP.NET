using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using REST.DataCore.Contract;

namespace REST.Linq2DbCore.Provider
{
    public class Linq2DbAsyncHelpers : IAsyncHelpers
    {
        public Task<T> SingleOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken token = default)
        {
            return queryable.SingleOrDefaultAsync(token);
        }

        public Task<T> SingleAsync<T>(IQueryable<T> queryable, CancellationToken token = default)
        {
            return queryable.SingleAsync(token);
        }

        public Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken token = default)
        {
            return queryable.FirstOrDefaultAsync(token);
        }

        public Task<T> FirstAsync<T>(IQueryable<T> queryable, CancellationToken token = default)
        {
            return queryable.FirstAsync(token);
        }

        public Task<T[]> ToArrayAsync<T>(IQueryable<T> queryable, CancellationToken token = default)
        {
            return queryable.ToArrayAsync(token);
        }

        public Task<int> CountAsync<T>(IQueryable<T> queryable, CancellationToken token = default)
        {
            return queryable.CountAsync(token);
        }

        public Task<long> LongCountAsync<T>(IQueryable<T> queryable,
            CancellationToken token = default)
        {
            return queryable.LongCountAsync(token);
        }
    }
}
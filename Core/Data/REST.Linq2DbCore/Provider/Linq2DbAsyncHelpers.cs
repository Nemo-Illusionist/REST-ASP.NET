using System.Linq;
using System.Threading.Tasks;
using LinqToDB;
using REST.DataCore.Manager;

namespace REST.Linq2DbCore.Provider
{
    public class Linq2DbAsyncHelpers : IAsyncHelpers
    {
        public Task<T> SingleOrDefaultAsync<T>(IQueryable<T> queryable)
        {
            return queryable.SingleOrDefaultAsync();
        }

        public Task<T> SingleAsync<T>(IQueryable<T> queryable)
        {
            return queryable.SingleAsync();
        }

        public Task<T[]> ToArrayAsync<T>(IQueryable<T> queryable)
        {
            return queryable.ToArrayAsync();
        }

        public Task<int> CountAsync<T>(IQueryable<T> queryable)
        {
            return queryable.CountAsync();
        }

        public Task<long> LongCountAsync<T>(IQueryable<T> queryable)
        {
            return queryable.LongCountAsync();
        }
    }
}
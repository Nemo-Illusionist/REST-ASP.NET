using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using REST.DataCore.Contract;

namespace REST.EfCore.Provider
{
    public class EfAsyncHelpers : IAsyncHelpers
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
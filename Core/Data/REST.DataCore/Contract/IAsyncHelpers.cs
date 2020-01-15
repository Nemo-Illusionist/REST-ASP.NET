using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace REST.DataCore.Contract
{
    public interface IAsyncHelpers
    {
        Task<T> SingleOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken token = default);
        Task<T> SingleAsync<T>(IQueryable<T> queryable, CancellationToken token = default);

        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken token = default);
        Task<T> FirstAsync<T>(IQueryable<T> queryable, CancellationToken token = default);

        Task<T[]> ToArrayAsync<T>(IQueryable<T> queryable, CancellationToken token = default);

        Task<int> CountAsync<T>(IQueryable<T> queryableForCount, CancellationToken token = default);
        Task<long> LongCountAsync<T>(IQueryable<T> queryable, CancellationToken token = default);
    }
}
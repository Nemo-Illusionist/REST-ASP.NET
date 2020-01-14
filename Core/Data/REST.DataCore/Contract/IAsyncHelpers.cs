using System.Linq;
using System.Threading.Tasks;

namespace REST.DataCore.Contract
{
    public interface IAsyncHelpers
    {
        Task<T> SingleOrDefaultAsync<T>(IQueryable<T> queryable);
        Task<T> SingleAsync<T>(IQueryable<T> queryable);
        Task<T[]> ToArrayAsync<T>(IQueryable<T> queryable);
        Task<int> CountAsync<T>(IQueryable<T> queryableForCount);
        Task<long> LongCountAsync<T>(IQueryable<T> queryable);
    }
}
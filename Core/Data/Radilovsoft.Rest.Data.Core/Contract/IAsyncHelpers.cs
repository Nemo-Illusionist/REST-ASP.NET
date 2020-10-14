using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Radilovsoft.Rest.Data.Core.Contract
{
    public interface IAsyncHelpers
    {
        Task<T> SingleOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);
        Task<T> SingleAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);

        Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);
        Task<T> FirstAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);

        Task<T[]> ToArrayAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);
        Task<List<T>> ToListAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);
        
        Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>(IQueryable<T> queryable, Func<T, TKey> keySelector, CancellationToken cancellationToken = default);
        Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>(IQueryable<T> queryable, Func<T, TKey> keySelector, Func<T, TElement> elementSelector, CancellationToken cancellationToken = default);

        Task<int> CountAsync<T>(IQueryable<T> queryableForCount, CancellationToken cancellationToken = default);
        Task<long> LongCountAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default);
    }
}
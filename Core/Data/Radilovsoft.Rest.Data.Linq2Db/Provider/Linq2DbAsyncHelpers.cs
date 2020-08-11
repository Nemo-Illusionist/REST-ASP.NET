using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using LinqToDB;
using Radilovsoft.Rest.Data.Core.Contract;

namespace Radilovsoft.Rest.Data.Linq2Db.Provider
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

        public Task<List<T>> ToListAsync<T>(IQueryable<T> queryable, CancellationToken token = default)
        {
            return queryable.ToListAsync(token);
        }

        public Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>(IQueryable<T> queryable, Func<T, TKey> keySelector, CancellationToken token = default)
        {
            return queryable.ToDictionaryAsync(keySelector, token);
        }

        public Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>(IQueryable<T> queryable, Func<T, TKey> keySelector, Func<T, TElement> elementSelector,
            CancellationToken token = default)
        {
            return queryable.ToDictionaryAsync(keySelector, elementSelector, token);
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
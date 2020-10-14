using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Radilovsoft.Rest.Data.Core.Contract;

namespace Radilovsoft.Rest.Data.Ef.Provider
{
    public class EfAsyncHelpers : IAsyncHelpers
    {
        public Task<T> SingleOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return queryable.SingleOrDefaultAsync(cancellationToken);
        }

        public Task<T> SingleAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return queryable.SingleAsync(cancellationToken);
        }

        public Task<T> FirstOrDefaultAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return queryable.FirstOrDefaultAsync(cancellationToken);
        }

        public Task<T> FirstAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return queryable.FirstAsync(cancellationToken);
        }

        public Task<T[]> ToArrayAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return queryable.ToArrayAsync(cancellationToken);
        }

        public Task<List<T>> ToListAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return queryable.ToListAsync(cancellationToken);
        }

        public Task<Dictionary<TKey, TElement>> ToDictionaryAsync<T, TKey, TElement>(
            IQueryable<T> queryable,
            Func<T, TKey> keySelector,
            Func<T, TElement> elementSelector,
            CancellationToken cancellationToken = default)
        {
            return queryable.ToDictionaryAsync(keySelector, elementSelector, cancellationToken);
        }

        public Task<Dictionary<TKey, T>> ToDictionaryAsync<T, TKey>(
            IQueryable<T> queryable,
            Func<T, TKey> keySelector,
            CancellationToken cancellationToken = default)
        {
            return queryable.ToDictionaryAsync(keySelector, cancellationToken);
        }

        public Task<int> CountAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return queryable.CountAsync(cancellationToken);
        }

        public Task<long> LongCountAsync<T>(IQueryable<T> queryable, CancellationToken cancellationToken = default)
        {
            return queryable.LongCountAsync(cancellationToken);
        }
    }
}
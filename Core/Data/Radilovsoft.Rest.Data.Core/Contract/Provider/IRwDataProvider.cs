using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Radilovsoft.Rest.Data.Core.Contract.Entity;

namespace Radilovsoft.Rest.Data.Core.Contract.Provider
{
    public interface IRwDataProvider : IRoDataProvider
    {
        Task<T> InsertAsync<T>(T entity, CancellationToken token = default) where T : class;

        Task BatchInsertAsync<T>(IEnumerable<T> entities, CancellationToken token = default) where T : class;

        Task<T> UpdateAsync<T>(T entity, CancellationToken token = default) where T : class;

        Task BatchUpdateAsync<T>(IEnumerable<T> entities, CancellationToken token = default) where T : class;

        Task SetDeleteAsync<T, TKey>(TKey id, CancellationToken token = default)
            where T : class, IDeletable, IEntity<TKey>
            where TKey : IComparable;

        Task BatchSetDeleteAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken token = default)
            where T : class, IDeletable, IEntity<TKey>
            where TKey : IComparable;

        Task SetUnDeleteAsync<T, TKey>(TKey id, CancellationToken token = default)
            where T : class, IDeletable, IEntity<TKey>
            where TKey : IComparable;

        Task BatchSetUnDeleteAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken token = default)
            where T : class, IDeletable, IEntity<TKey>
            where TKey : IComparable;
    }
}
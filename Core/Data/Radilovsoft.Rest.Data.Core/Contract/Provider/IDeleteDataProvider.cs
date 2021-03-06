using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Radilovsoft.Rest.Data.Core.Contract.Entity;

namespace Radilovsoft.Rest.Data.Core.Contract.Provider
{
    public interface IDeleteDataProvider
    {
        Task DeleteAsync<T>(T entity, CancellationToken token = default) where T : class;
        Task BatchDeleteAsync<T>(IEnumerable<T> entities, CancellationToken token = default) where T : class;

        Task DeleteByIdAsync<T, TKey>(TKey id, CancellationToken token = default)
            where T : class, IEntity<TKey> where TKey : IComparable;

        Task BatchDeleteByIdsAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken token = default)
            where T : class, IEntity<TKey> where TKey : IComparable;
    }
}
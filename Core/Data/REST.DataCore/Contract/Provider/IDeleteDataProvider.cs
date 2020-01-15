using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using REST.DataCore.Contract.Entity;

namespace REST.DataCore.Contract.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IDeleteDataProvider
    {
        Task DeleteAsync<T>(T entity, CancellationToken token = default) where T : class, IEntity;
        Task BatchDeleteAsync<T>(IEnumerable<T> entities, CancellationToken token = default) where T : class, IEntity;

        Task DeleteByIdAsync<T, TKey>(TKey id, CancellationToken token = default)
            where T : class, IEntity, IEntity<TKey> where TKey : IComparable;

        Task BatchDeleteByIdsAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken token = default)
            where T : class, IEntity, IEntity<TKey> where TKey : IComparable;
    }
}
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using REST.DataCore.Contract.Entity;

namespace REST.DataCore.Contract.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IRwDataProvider : IRoDataProvider
    {
        Task<T> InsertAsync<T>(T entity, CancellationToken token = default) where T : class, IEntity;

        Task BatchInsertAsync<T>(IEnumerable<T> entities, CancellationToken token = default) where T : class, IEntity;

        Task<T> UpdateAsync<T>(T entity, CancellationToken token = default) where T : class, IEntity;

        Task BatchUpdateAsync<T>(IEnumerable<T> entities, CancellationToken token = default) where T : class, IEntity;

        Task SetDeleteAsync<T, TKey>(TKey id, CancellationToken token = default)
            where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable;

        Task BatchSetDeleteAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken token = default)
            where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable;

        Task SetUnDeleteAsync<T, TKey>(TKey id, CancellationToken token = default)
            where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable;

        Task BatchSetUnDeleteAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken token = default)
            where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable;
    }
}
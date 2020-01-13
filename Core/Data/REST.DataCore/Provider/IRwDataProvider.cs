using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using REST.DataCore.EntityContract;

namespace REST.DataCore.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IRwDataProvider : IRoDataProvider
    {
        Task<T> InsertAsync<T>(T entity) where T : class, IEntity;

        /// <summary>
        /// Пакетная вставка записей в БД.
        /// </summary>
        Task BatchInsertAsync<T>(IEnumerable<T> entities) where T : class, IEntity;

        Task<T> UpdateAsync<T>(T entity, bool ignoreSystemProps = true) where T : class, IEntity;
        Task BatchUpdateAsync<T>(IEnumerable<T> entities, bool ignoreSystemProps = true) where T : class, IEntity;

        Task SetDeleteAsync<T, TKey>(TKey id) where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable;

        Task BatchSetDeleteAsync<T, TKey>(IEnumerable<TKey> ids) where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable;

        Task SetUnDeleteAsync<T, TKey>(TKey id) where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable;

        Task BatchSetUnDeleteAsync<T, TKey>(IEnumerable<TKey> ids) where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable;
    }
}
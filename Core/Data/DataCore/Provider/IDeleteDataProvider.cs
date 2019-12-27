using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DataCore.EntityContract;
using JetBrains.Annotations;

namespace DataCore.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IDeleteDataProvider
    {
        Task DeleteAsync<T>(T entity) where T : class, IEntity;
        Task BatchDeleteAsync<T>(IEnumerable<T> entities) where T : class, IEntity;

        Task DeleteByIdAsync<T, TKey>(TKey id) where T : class, IEntity, IEntity<TKey> where TKey : IComparable;

        Task BatchDeleteByIdsAsync<T, TKey>(IEnumerable<TKey> ids)
            where T : class, IEntity, IEntity<TKey> where TKey : IComparable;
    }
}
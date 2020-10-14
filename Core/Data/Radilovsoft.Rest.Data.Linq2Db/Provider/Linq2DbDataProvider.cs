using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LinqToDB;
using LinqToDB.Data;
using LinqToDB.Linq;
using Radilovsoft.Rest.Data.Core.Contract;
using Radilovsoft.Rest.Data.Core.Contract.Entity;
using Radilovsoft.Rest.Data.Core.Contract.Provider;

namespace Radilovsoft.Rest.Data.Linq2Db.Provider
{
    public class Linq2DbDataProvider : IDataProvider
    {
        private readonly DataConnection _dataConnection;

        private readonly IDataExceptionManager _exceptionManager;

        public Linq2DbDataProvider(
            DataConnection dataConnection,
            IDataExceptionManager exceptionManager)
        {
            _dataConnection = dataConnection ?? throw new ArgumentNullException(nameof(dataConnection));
            _exceptionManager = exceptionManager ?? throw new ArgumentNullException(nameof(exceptionManager));
        }

        public IDataTransaction Transaction()
        {
            return new Linq2DbDataTransactionAdapter(_dataConnection.BeginTransaction());
        }

        public IDataTransaction Transaction(IsolationLevel isolationLevel)
        {
            return new Linq2DbDataTransactionAdapter(_dataConnection.BeginTransaction(isolationLevel));
        }


        public IQueryable<T> GetQueryable<T>() where T : class
        {
            return _dataConnection.GetTable<T>();
        }

        #region Modify

        public Task<T> InsertAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            return ExecuteCommand(async state =>
            {
                SetSystemProps(state.entity);
                await _dataConnection.InsertAsync(state.entity, token: state.token).ConfigureAwait(false);
                return state.entity;
            }, (entity, token: cancellationToken));
        }

        public Task<T> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default)
            where T : class
        {
            return ExecuteCommand(async state =>
            {
                SetUpdatedUtc(state.entity);
                await _dataConnection.UpdateAsync(state.entity, token: state.token).ConfigureAwait(false);
                return state.entity;
            }, (entity, token: cancellationToken));
        }

        public Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            return ExecuteCommand(state => _dataConnection.DeleteAsync(state.entity, token: state.token),
                (entity, token: cancellationToken));
        }

        public Task DeleteByIdAsync<T, TKey>(TKey id, CancellationToken cancellationToken = default)
            where T : class, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(
                state =>
                {
                    return _dataConnection.GetTable<T>().Where(x => x.Id.Equals(state.id)).DeleteAsync(state.token);
                }, (id, token: cancellationToken));
        }

        public Task SetDeleteAsync<T, TKey>(TKey id, CancellationToken cancellationToken = default)
            where T : class, IDeletable, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(state =>
            {
                var queryable = _dataConnection.GetTable<T>()
                    .Where(x => x.Id.Equals(state.id))
                    .Set(x => x.DeletedUtc, DateTime.UtcNow);

                queryable = SetUpdateUtc(queryable);

                return queryable.UpdateAsync(state.token);
            }, (id, token: cancellationToken));
        }

        public Task SetUnDeleteAsync<T, TKey>(TKey id, CancellationToken cancellationToken = default)
            where T : class, IDeletable, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(state =>
            {
                var queryable = _dataConnection.GetTable<T>()
                    .Where(x => x.Id.Equals(state.id))
                    .Set(x => x.DeletedUtc, (DateTime?) null);

                queryable = SetUpdateUtc(queryable);

                return queryable.UpdateAsync(state.token);
            }, (id, token: cancellationToken));
        }

        #endregion

        #region BatchModify

        public Task BatchInsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            where T : class
        {
            return ExecuteCommand(states =>
            {
                states.entities = states.entities.Select(SetSystemProps);
                return Task.Factory.StartNew(() => _dataConnection.BulkCopy(states.entities), cancellationToken,
                    TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }, (entities, token: cancellationToken));
        }

        public Task BatchUpdateAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            where T : class
        {
            return ExecuteCommand(async states =>
            {
                //todo: optimize
                foreach (var entity in states.entities)
                {
                    SetUpdatedUtc(entity);
                    await _dataConnection.UpdateAsync(entity, token: states.token).ConfigureAwait(false);
                }

                return new object();
            }, (entities, token: cancellationToken));
        }

        public Task BatchDeleteAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            where T : class
        {
            return ExecuteCommand(
                state =>
                {
                    return _dataConnection.GetTable<T>().Where(x => state.entities.Any(x.Equals))
                        .DeleteAsync(state.token);
                },
                (entities, token: cancellationToken));
        }

        public Task BatchDeleteByIdsAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
            where T : class, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(
                state =>
                {
                    return _dataConnection.GetTable<T>().Where(x => state.ids.Contains(x.Id)).DeleteAsync(state.token);
                }, (ids, token: cancellationToken));
        }

        public Task BatchSetDeleteAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
            where T : class, IDeletable, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(state =>
            {
                var queryable = _dataConnection.GetTable<T>()
                    .Where(x => state.ids.Contains(x.Id))
                    .Set(x => x.DeletedUtc, DateTime.UtcNow);

                queryable = SetUpdateUtc(queryable);

                return queryable.UpdateAsync(state.token);
            }, (ids, token: cancellationToken));
        }

        public Task BatchSetUnDeleteAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
            where T : class, IDeletable, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(state =>
            {
                var queryable = _dataConnection.GetTable<T>()
                    .Where(x => state.ids.Contains(x.Id))
                    .Set(x => x.DeletedUtc, (DateTime?) null);

                queryable = SetUpdateUtc(queryable);

                return queryable.UpdateAsync(state.token);
            }, (ids, token: cancellationToken));
        }

        #endregion

        private IUpdatable<T> SetUpdateUtc<T>(IUpdatable<T> updatable)
        {
            if (typeof(T).IsAssignableFrom(typeof(IUpdatedUtc)))
            {
                return updatable.Set(x => ((IUpdatedUtc) x).UpdatedUtc, DateTime.UtcNow);
            }
            else
            {
                return updatable;
            }
        }

        private T SetSystemProps<T>(T entity) where T : class
        {
            if (entity is ICreatedUtc createdUtc)
            {
                createdUtc.CreatedUtc = DateTime.UtcNow;
            }

            entity = SetUpdatedUtc(entity);
            return entity;
        }

        private T SetUpdatedUtc<T>(T entity) where T : class
        {
            if (entity is IUpdatedUtc updatedUtc)
            {
                updatedUtc.UpdatedUtc = DateTime.UtcNow;
            }

            return entity;
        }

        private async Task<T> ExecuteCommand<T, TState>(Func<TState, Task<T>> func, TState state)
        {
            try
            {
                return await func(state).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw _exceptionManager.Normalize(exception);
            }
        }
    }
}
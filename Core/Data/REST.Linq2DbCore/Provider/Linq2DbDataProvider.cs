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
using REST.DataCore.Contract;
using REST.DataCore.Contract.Entity;
using REST.DataCore.Contract.Provider;

namespace REST.Linq2DbCore.Provider
{
    public class Linq2DbDataProvider : IDataProvider, ISafeExecuteProvider
    {
        private readonly DataConnection _dataConnection;
        private readonly IDataExceptionManager _exceptionManager;

        public Linq2DbDataProvider(DataConnection dataConnection, [NotNull] IDataExceptionManager exceptionManager)
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


        public IQueryable<T> GetQueryable<T>() where T : class, IEntity
        {
            return _dataConnection.GetTable<T>();
        }

        #region Modify

        public Task<T> InsertAsync<T>(T entity, CancellationToken token = default) where T : class, IEntity
        {
            return ExecuteCommand(async state =>
            {
                state.entity = SetSystemProps(state.entity);
                await _dataConnection.InsertAsync(state.entity, token: state.token).ConfigureAwait(false);
                return state.entity;
            }, (entity, token));
        }

        public Task<T> UpdateAsync<T>(T entity, bool ignoreSystemProps = true, CancellationToken token = default)
            where T : class, IEntity
        {
            return ExecuteCommand(async state =>
            {
                SetSystemPropsForUpdate(state.entity, state.ignoreSystemProps);
                await _dataConnection.UpdateAsync(state.entity, token: state.token).ConfigureAwait(false);
                return state.entity;
            }, (entity, ignoreSystemProps, token));
        }

        public Task DeleteAsync<T>(T entity, CancellationToken token = default) where T : class, IEntity
        {
            return ExecuteCommand(state => _dataConnection.DeleteAsync(state.entity, token: state.token),
                (entity, token));
        }

        public Task DeleteByIdAsync<T, TKey>(TKey id, CancellationToken token = default)
            where T : class, IEntity, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(
                state =>
                {
                    return _dataConnection.GetTable<T>().Where(x => x.Id.Equals(state.id)).DeleteAsync(state.token);
                }, (id, token));
        }

        public Task SetDeleteAsync<T, TKey>(TKey id, CancellationToken token = default)
            where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(state =>
            {
                var queryable = _dataConnection.GetTable<T>()
                    .Where(x => x.Id.Equals(state.id))
                    .Set(x => x.DeletedUtc, DateTime.UtcNow);

                queryable = SetUpdateUtc(queryable);

                return queryable.UpdateAsync(state.token);
            }, (id, token));
        }

        public Task SetUnDeleteAsync<T, TKey>(TKey id, CancellationToken token = default)
            where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(state =>
            {
                var queryable = _dataConnection.GetTable<T>()
                    .Where(x => x.Id.Equals(state.id))
                    .Set(x => x.DeletedUtc, (DateTime?) null);

                queryable = SetUpdateUtc(queryable);

                return queryable.UpdateAsync(state.token);
            }, (id, token));
        }

        #endregion

        #region BatchModify

        public Task BatchInsertAsync<T>(IEnumerable<T> entities, CancellationToken token = default)
            where T : class, IEntity
        {
            return ExecuteCommand(states =>
            {
                states.entities = states.entities.Select(SetSystemProps);
                return Task.Factory.StartNew(() => _dataConnection.BulkCopy(states.entities), token,
                    TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }, (entities, token));
        }

        public Task BatchUpdateAsync<T>(IEnumerable<T> entities, bool ignoreSystemProps = true,
            CancellationToken token = default) where T : class, IEntity
        {
            return ExecuteCommand(async states =>
            {
                states.entities = states.entities.Select(x => SetSystemPropsForUpdate(x, states.ignoreSystemProps));
                //todo: optimize
                foreach (var entity in states.entities)
                {
                    await _dataConnection.UpdateAsync(entity, token: states.token).ConfigureAwait(false);
                }

                return new object();
            }, (entities, ignoreSystemProps, token));
        }

        public Task BatchDeleteAsync<T>(IEnumerable<T> entities, CancellationToken token = default)
            where T : class, IEntity
        {
            return ExecuteCommand(
                state =>
                {
                    return _dataConnection.GetTable<T>().Where(x => state.entities.Any(x.Equals))
                        .DeleteAsync(state.token);
                },
                (entities, token));
        }

        public Task BatchDeleteByIdsAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken token = default)
            where T : class, IEntity, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(
                state =>
                {
                    return _dataConnection.GetTable<T>().Where(x => state.ids.Contains(x.Id)).DeleteAsync(state.token);
                }, (ids, token));
        }

        public Task BatchSetDeleteAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken token = default)
            where T : class, IEntity, IDeletable, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(state =>
            {
                var queryable = _dataConnection.GetTable<T>()
                    .Where(x => state.ids.Contains(x.Id))
                    .Set(x => x.DeletedUtc, DateTime.UtcNow);

                queryable = SetUpdateUtc(queryable);

                return queryable.UpdateAsync(state.token);
            }, (ids, token));
        }

        public Task BatchSetUnDeleteAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken token = default)
            where T : class, IEntity, IDeletable, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(state =>
            {
                var queryable = _dataConnection.GetTable<T>()
                    .Where(x => state.ids.Contains(x.Id))
                    .Set(x => x.DeletedUtc, (DateTime?) null);

                queryable = SetUpdateUtc(queryable);

                return queryable.UpdateAsync(state.token);
            }, (ids, token));
        }

        #endregion

        public async Task<T> SafeExecuteAsync<T>([InstantHandle] Func<IDataProvider, CancellationToken, Task<T>> action,
            IsolationLevel level = IsolationLevel.RepeatableRead, int retryCount = 3, CancellationToken token = default)
        {
            var result = default(T);

            async Task Wrapper(IDataProvider db, CancellationToken cancellationToken) =>
                result = await action(db, cancellationToken).ConfigureAwait(false);

            await SafeExecuteAsync(Wrapper, level, retryCount, token).ConfigureAwait(false);

            return result;
        }

        public async Task SafeExecuteAsync([InstantHandle] Func<IDataProvider, CancellationToken, Task> action,
            IsolationLevel level = IsolationLevel.RepeatableRead, int retryCount = 3, CancellationToken token = default)
        {
            var count = 0;
            while (true)
            {
                try
                {
                    await using var transaction = Transaction(level);
                    await action(this, token).ConfigureAwait(false);
                    await transaction.CommitAsync(token).ConfigureAwait(false);
                    break;
                }
                catch (Exception exception)
                {
                    if (_exceptionManager.IsConcurrentModifyException(exception) && ++count >= retryCount) throw;

                    await Task.Delay(TimeSpan.FromSeconds(1), token).ConfigureAwait(false);
                }
            }
        }

        private IUpdatable<T> SetUpdateUtc<T>(IUpdatable<T> updatable)
        {
            if (typeof(T).IsAssignableFrom(typeof(IUpdatedUtc)))
            {
                // ReSharper disable once SuspiciousTypeConversion.Global
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

            if (entity is IUpdatedUtc updatedUtc)
            {
                updatedUtc.UpdatedUtc = DateTime.UtcNow;
            }

            return entity;
        }

        private T SetSystemPropsForUpdate<T>(T entity, bool ignoreSystemProps) where T : class
        {
            if (entity is IUpdatedUtc updatedUtc)
            {
                updatedUtc.UpdatedUtc = DateTime.UtcNow;
            }

            if (ignoreSystemProps)
            {
                //todo 
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
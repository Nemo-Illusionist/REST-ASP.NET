using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataCore.EntityContract;
using DataCore.Manager;
using DataCore.Provider;
using JetBrains.Annotations;
using LinqToDB;
using LinqToDB.Data;

namespace Linq2DbCodre.Provider
{
    public class Linq2DbDataProvider : IDataProvider, ISafeExecuteProvider
    {
        private readonly DataConnection _dataConnection;
        private readonly IDbExceptionManager _exceptionManager;

        public Linq2DbDataProvider(DataConnection dataConnection, [NotNull] IDbExceptionManager exceptionManager)
        {
            _dataConnection = dataConnection ?? throw new ArgumentNullException(nameof(dataConnection));
            _exceptionManager = exceptionManager ?? throw new ArgumentNullException(nameof(exceptionManager));
        }

        public IDataTransaction Transaction()
        {
            return new DataTransactionAdapter(_dataConnection.BeginTransaction());
        }

        public IDataTransaction Transaction(IsolationLevel isolationLevel)
        {
            return new DataTransactionAdapter(_dataConnection.BeginTransaction(isolationLevel));
        }


        public IQueryable<T> GetQueryable<T>() where T : class, IEntity
        {
            return _dataConnection.GetTable<T>();
        }

        #region Modify

        public Task<T> InsertAsync<T>(T entity) where T : class, IEntity
        {
            return ExecuteCommand(async state =>
            {
                state = SetSystemProps(state);
                await _dataConnection.InsertAsync(state).ConfigureAwait(false);
                return state;
            }, entity);
        }

        public Task<T> UpdateAsync<T>(T entity, bool ignoreSystemProps = true) where T : class, IEntity
        {
            return ExecuteCommand(async state =>
            {
                SetSystemPropsForUpdate(state.entity, state.ignoreSystemProps);
                await _dataConnection.UpdateAsync(state.entity).ConfigureAwait(false);
                return state.entity;
            }, (entity, ignoreSystemProps));
        }

        public Task DeleteAsync<T>(T entity) where T : class, IEntity
        {
            return ExecuteCommand(state => _dataConnection.DeleteAsync(state), entity);
        }

        public Task DeleteByIdAsync<T, TKey>(TKey id) where T : class, IEntity, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(
                state => { return _dataConnection.GetTable<T>().Where(x => x.Id.Equals(state)).DeleteAsync(); }, id);
        }

        public Task SetDeleteAsync<T, TKey>(TKey id) where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(state =>
            {
                var queryable = _dataConnection.GetTable<T>()
                    .Where(x => x.Id.Equals(state))
                    .Set(x => x.DeletedUtc, DateTime.UtcNow);

                if (typeof(T).IsAssignableFrom(typeof(IUpdatedUtc)))
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    queryable = queryable.Set(x => ((IUpdatedUtc) x).UpdatedUtc, DateTime.UtcNow);
                }

                return queryable.UpdateAsync();
            }, id);
        }

        #endregion

        #region BatchModify

        public Task BatchInsertAsync<T>(IEnumerable<T> entities) where T : class, IEntity
        {
            return ExecuteCommand(states =>
            {
                states = states.Select(SetSystemProps);
                return Task.Factory.StartNew(() => _dataConnection.BulkCopy(states),
                    CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            }, entities);
        }

        public Task BatchUpdateAsync<T>(IEnumerable<T> entities, bool ignoreSystemProps = true) where T : class, IEntity
        {
            return ExecuteCommand(async states =>
            {
                states.entities = states.entities.Select(x => SetSystemPropsForUpdate(x, states.ignoreSystemProps));
                //todo: optimize
                foreach (var entity in states.entities)
                {
                    await _dataConnection.UpdateAsync(entity).ConfigureAwait(false);
                }

                return new object();
            }, (entities, ignoreSystemProps));
        }

        public Task BatchDeleteAsync<T>(IEnumerable<T> entities) where T : class, IEntity
        {
            return ExecuteCommand(
                state => { return _dataConnection.GetTable<T>().Where(x => state.Any(x.Equals)).DeleteAsync(); },
                entities);
        }

        public Task BatchDeleteByIdsAsync<T, TKey>(IEnumerable<TKey> ids) where T : class, IEntity, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(
                state => { return _dataConnection.GetTable<T>().Where(x => state.Contains(x.Id)).DeleteAsync(); }, ids);
        }

        public Task BatchSetDeleteAsync<T, TKey>(IEnumerable<TKey> ids)
            where T : class, IEntity, IDeletable, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(state =>
            {
                var queryable = _dataConnection.GetTable<T>()
                    .Where(x => ids.Contains(x.Id))
                    .Set(x => x.DeletedUtc, DateTime.UtcNow);

                if (typeof(T).IsAssignableFrom(typeof(IUpdatedUtc)))
                {
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    queryable = queryable.Set(x => ((IUpdatedUtc) x).UpdatedUtc, DateTime.UtcNow);
                }

                return queryable.UpdateAsync();
            }, ids);
        }

        #endregion


        public async Task<T> SafeExecuteAsync<T>(Func<IDataProvider, Task<T>> action,
            IsolationLevel level = IsolationLevel.RepeatableRead, int retryCount = 3)
        {
            var result = default(T);
            async Task Wrapper(IDataProvider db) => result = await action(db).ConfigureAwait(false);

            await SafeExecuteAsync(Wrapper, level, retryCount).ConfigureAwait(false);

            return result;
        }

        public async Task SafeExecuteAsync(Func<IDataProvider, Task> action,
            IsolationLevel level = IsolationLevel.RepeatableRead, int retryCount = 3)
        {
            var count = 0;
            while (true)
            {
                try
                {
                    using var transaction = Transaction(level);
                    await action(this).ConfigureAwait(false);
                    transaction.Commit();
                    break;
                }
                catch (Exception exception)
                {
                    if (!_exceptionManager.IsConcurrentModifyException(exception) || ++count >= retryCount) throw;

                    await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
                }
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using REST.DataCore.Contract;
using REST.DataCore.Contract.Entity;
using REST.DataCore.Contract.Provider;
using REST.DataCore.Manager;
using REST.EfCore.Context;

namespace REST.EfCore.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class EfDataProvider : IDataProvider, ISafeExecuteProvider
    {
        private readonly ResetDbContext _dbContext;
        private readonly IDataExceptionManager _dataExceptionManager;

        public EfDataProvider([NotNull] ResetDbContext connection) : this(connection, new DefaultDataExceptionManager())
        {
        }

        public EfDataProvider([NotNull] ResetDbContext connection, [NotNull] IDataExceptionManager dataExceptionManager)
        {
            _dbContext = connection ?? throw new ArgumentNullException(nameof(connection));
            _dataExceptionManager =
                dataExceptionManager ?? throw new ArgumentNullException(nameof(dataExceptionManager));
        }

        public IDataTransaction Transaction()
        {
            var transaction = _dbContext.Database.BeginTransaction();
            return new EfDataTransactionAdapter(transaction);
        }

        public IDataTransaction Transaction(IsolationLevel isolationLevel)
        {
            var transaction = _dbContext.Database.BeginTransaction(isolationLevel);
            return new EfDataTransactionAdapter(transaction);
        }

        public IQueryable<T> GetQueryable<T>() where T : class, IEntity
        {
            return _dbContext.Set<T>().AsQueryable().AsNoTracking();
        }

        #region Modify

        public Task<T> InsertAsync<T>(T entity, CancellationToken token = default) where T : class, IEntity
        {
            return ExecuteCommand(async state =>
            {
                Add(state.entity);
                await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
                return state.entity;
            }, (entity, token));
        }

        public Task<T> UpdateAsync<T>(T entity, bool ignoreSystemProps = true, CancellationToken token = default)
            where T : class, IEntity
        {
            return ExecuteCommand(async state =>
            {
                UpdateEntity(state.entity, state.ignoreSystemProps);
                await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
                return state.entity;
            }, (entity, ignoreSystemProps, token));
        }

        public Task DeleteAsync<T>(T entity, CancellationToken token = default) where T : class, IEntity
        {
            return ExecuteCommand(state =>
            {
                _dbContext.Set<T>().Remove(state.entity);
                return _dbContext.SaveChangesAsync(state.token);
            }, (entity, token));
        }

        public Task DeleteByIdAsync<T, TKey>(TKey id, CancellationToken token = default)
            where T : class, IEntity, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entity = await _dbContext.Set<T>().Where(t => state.id.Equals(t.Id)).SingleAsync(state.token)
                    .ConfigureAwait(false);
                _dbContext.Set<T>().Remove(entity);
                return await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
            }, (id, token));
        }

        public Task SetDeleteAsync<T, TKey>(TKey id, CancellationToken token = default)
            where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entity = await _dbContext.Set<T>().Where(t => state.id.Equals(t.Id)).SingleAsync(state.token)
                    .ConfigureAwait(false);
                entity.DeletedUtc = DateTime.UtcNow;
                UpdateEntity(entity, false);
                return await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
            }, (id, token));
        }

        public Task SetUnDeleteAsync<T, TKey>(TKey id, CancellationToken token = default)
            where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entity = await _dbContext.Set<T>().Where(t => state.id.Equals(t.Id)).SingleAsync(state.token)
                    .ConfigureAwait(false);
                entity.DeletedUtc = null;
                UpdateEntity(entity, false);
                return await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
            }, (id, token));
        }

        #endregion

        #region BatchModify

        public Task BatchInsertAsync<T>(IEnumerable<T> entities, CancellationToken token = default)
            where T : class, IEntity
        {
            return ExecuteCommand(state =>
            {
                foreach (var entity in state.entities)
                {
                    Add(entity);
                }

                return _dbContext.SaveChangesAsync(state.token);
            }, (entities, token));
        }

        public Task BatchUpdateAsync<T>(IEnumerable<T> entities, bool ignoreSystemProps = true,
            CancellationToken token = default) where T : class, IEntity
        {
            return ExecuteCommand(state =>
            {
                foreach (var entity in state.entities)
                {
                    UpdateEntity(entity, state.ignoreSystemProps);
                }

                return _dbContext.SaveChangesAsync(state.token);
            }, (entities, ignoreSystemProps, token));
        }

        public Task BatchDeleteAsync<T>(IEnumerable<T> entities, CancellationToken token = default)
            where T : class, IEntity
        {
            return ExecuteCommand(state =>
            {
                _dbContext.Set<T>().RemoveRange(state.entities);
                return _dbContext.SaveChangesAsync(state.token);
            }, (entities, token));
        }

        public Task BatchDeleteByIdsAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken token = default)
            where T : class, IEntity, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entity = await _dbContext.Set<T>().Where(t => state.ids.Contains(t.Id)).ToArrayAsync(state.token)
                    .ConfigureAwait(false);
                _dbContext.Set<T>().RemoveRange(entity);
                return await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
            }, (ids, token));
        }

        public Task BatchSetDeleteAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken token = default)
            where T : class, IEntity, IDeletable, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entities = await _dbContext.Set<T>().Where(t => state.ids.Contains(t.Id)).ToArrayAsync(state.token)
                    .ConfigureAwait(false);

                foreach (var entity in entities)
                {
                    entity.DeletedUtc = DateTime.UtcNow;
                    UpdateEntity(entity, false);
                }

                return await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
            }, (ids, token));
        }

        public Task BatchSetUnDeleteAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken token = default)
            where T : class, IEntity, IDeletable, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entities = await _dbContext.Set<T>().Where(t => state.ids.Contains(t.Id)).ToArrayAsync(state.token)
                    .ConfigureAwait(false);

                foreach (var entity in entities)
                {
                    entity.DeletedUtc = null;
                    UpdateEntity(entity, false);
                }

                return await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
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
                    _dbContext.Reset();

                    if (_dataExceptionManager.IsConcurrentModifyException(exception) && ++count >= retryCount) throw;

                    await Task.Delay(TimeSpan.FromSeconds(1), token).ConfigureAwait(false);
                }
            }
        }

        private void Add<T>(T entity) where T : class
        {
            if (entity is ICreatedUtc createdUtc)
            {
                createdUtc.CreatedUtc = DateTime.UtcNow;
            }

            if (entity is IUpdatedUtc updatedUtc)
            {
                updatedUtc.UpdatedUtc = DateTime.UtcNow;
            }

            _dbContext.Set<T>().Add(entity);
        }

        private void UpdateEntity<T>(T entity, bool ignoreSystemProps) where T : class
        {
            if (entity is IUpdatedUtc updatedUtc)
            {
                updatedUtc.UpdatedUtc = DateTime.UtcNow;
            }

            EntityEntry<T> entityEntry = _dbContext.Entry(entity);
            entityEntry.State = EntityState.Modified;

            if (ignoreSystemProps)
            {
                if (entity is IDeletable)
                {
                    entityEntry.Property(nameof(IDeletable.DeletedUtc)).IsModified = false;
                }

                if (entity is ICreatedUtc)
                {
                    entityEntry.Property(nameof(ICreatedUtc.CreatedUtc)).IsModified = false;
                }
            }
        }

        private async Task<T> ExecuteCommand<T, TState>(Func<TState, Task<T>> func, TState state)
        {
            try
            {
                return await func(state).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                throw _dataExceptionManager.Normalize(exception);
            }
        }
    }
}
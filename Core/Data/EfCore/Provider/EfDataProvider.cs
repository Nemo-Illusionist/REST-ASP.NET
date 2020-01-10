using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DataCore.EntityContract;
using DataCore.Manager;
using DataCore.Provider;
using EfCore.Context;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace EfCore.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class EfDataProvider : IDataProvider, ISafeExecuteProvider
    {
        private readonly ResetDbContext _dbContext;
        private readonly IDbExceptionManager _dbExceptionManager;

        public EfDataProvider([NotNull] ResetDbContext connection) : this(connection, new DefaultDbExceptionManager())
        {
        }

        public EfDataProvider([NotNull] ResetDbContext connection, [NotNull] IDbExceptionManager dbExceptionManager)
        {
            _dbContext = connection ?? throw new ArgumentNullException(nameof(connection));
            _dbExceptionManager = dbExceptionManager ?? throw new ArgumentNullException(nameof(dbExceptionManager));
        }

        public IDataTransaction Transaction()
        {
            var transaction = _dbContext.Database.BeginTransaction();
            return new DataTransactionAdapter(transaction);
        }

        public IDataTransaction Transaction(IsolationLevel isolationLevel)
        {
            var transaction = _dbContext.Database.BeginTransaction(isolationLevel);
            return new DataTransactionAdapter(transaction);
        }

        public IQueryable<T> GetQueryable<T>() where T : class, IEntity
        {
            return _dbContext.Set<T>().AsQueryable().AsNoTracking();
        }

        #region Modify

        public Task<T> InsertAsync<T>(T entity) where T : class, IEntity
        {
            return ExecuteCommand(async state =>
            {
                Add(state);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return state;
            }, entity);
        }

        public Task<T> UpdateAsync<T>(T entity, bool ignoreSystemProps = true) where T : class, IEntity
        {
            return ExecuteCommand(async state =>
            {
                UpdateEntity(state.entity, state.ignoreSystemProps);
                await _dbContext.SaveChangesAsync().ConfigureAwait(false);
                return state.entity;
            }, (entity, ignoreSystemProps));
        }

        public Task DeleteAsync<T>(T entity) where T : class, IEntity
        {
            return ExecuteCommand(state =>
            {
                _dbContext.Set<T>().Remove(state);
                return _dbContext.SaveChangesAsync();
            }, entity);
        }

        public Task DeleteByIdAsync<T, TKey>(TKey id) where T : class, IEntity, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entity = await _dbContext.Set<T>().Where(t => state.Equals(t.Id)).SingleAsync()
                    .ConfigureAwait(false);
                _dbContext.Set<T>().Remove(entity);
                return await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }, id);
        }

        public Task SetDeleteAsync<T, TKey>(TKey id) where T : class, IEntity, IDeletable, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entity = await _dbContext.Set<T>().Where(t => state.Equals(t.Id)).SingleAsync()
                    .ConfigureAwait(false);
                entity.DeletedUtc = DateTime.UtcNow;
                UpdateEntity(entity, false);
                return await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }, id);
        }

        #endregion

        #region BatchModify

        public Task BatchInsertAsync<T>(IEnumerable<T> entities) where T : class, IEntity
        {
            return ExecuteCommand(state =>
            {
                foreach (var entity in state)
                {
                    Add(entity);
                }

                return _dbContext.SaveChangesAsync();
            }, entities);
        }

        public Task BatchUpdateAsync<T>(IEnumerable<T> entities, bool ignoreSystemProps = true) where T : class, IEntity
        {
            return ExecuteCommand(state =>
            {
                foreach (var entity in state.entities)
                {
                    UpdateEntity(entity, state.ignoreSystemProps);
                }

                return _dbContext.SaveChangesAsync();
            }, (entities, ignoreSystemProps));
        }

        public Task BatchDeleteAsync<T>(IEnumerable<T> entities) where T : class, IEntity
        {
            return ExecuteCommand(state =>
            {
                _dbContext.Set<T>().RemoveRange(state);
                return _dbContext.SaveChangesAsync();
            }, entities);
        }

        public Task BatchDeleteByIdsAsync<T, TKey>(IEnumerable<TKey> ids) where T : class, IEntity, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entity = await _dbContext.Set<T>().Where(t => state.Contains(t.Id)).ToArrayAsync()
                    .ConfigureAwait(false);
                _dbContext.Set<T>().RemoveRange(entity);
                return await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }, ids);
        }

        public Task BatchSetDeleteAsync<T, TKey>(IEnumerable<TKey> ids)
            where T : class, IEntity, IDeletable, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entities = await _dbContext.Set<T>().Where(t => state.Contains(t.Id)).ToArrayAsync()
                    .ConfigureAwait(false);

                foreach (var entity in entities)
                {
                    entity.DeletedUtc = DateTime.UtcNow;
                    UpdateEntity(entity, false);
                }

                return await _dbContext.SaveChangesAsync().ConfigureAwait(false);
            }, ids);
        }

        #endregion

        public async Task<T> SafeExecuteAsync<T>([InstantHandle] Func<IDataProvider, Task<T>> action,
            IsolationLevel level = IsolationLevel.RepeatableRead, int retryCount = 3)
        {
            var result = default(T);
            async Task Wrapper(IDataProvider db) => result = await action(db).ConfigureAwait(false);

            await SafeExecuteAsync(Wrapper, level, retryCount).ConfigureAwait(false);

            return result;
        }

        public async Task SafeExecuteAsync([InstantHandle] Func<IDataProvider, Task> action,
            IsolationLevel level = IsolationLevel.RepeatableRead, int retryCount = 3)
        {
            var count = 0;
            while (true)
            {
                try
                {
                    await using var transaction = Transaction(level);
                    await action(this).ConfigureAwait(false);
                    transaction.Commit();
                    break;
                }
                catch (Exception exception)
                {
                    _dbContext.Reset();

                    if (!_dbExceptionManager.IsConcurrentModifyException(exception) || ++count >= retryCount) throw;

                    await Task.Delay(TimeSpan.FromSeconds(1)).ConfigureAwait(false);
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
                throw _dbExceptionManager.Normalize(exception);
            }
        }
    }
}
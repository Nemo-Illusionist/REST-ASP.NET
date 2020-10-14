using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Radilovsoft.Rest.Data.Core.Contract;
using Radilovsoft.Rest.Data.Core.Contract.Entity;
using Radilovsoft.Rest.Data.Core.Contract.Provider;
using Radilovsoft.Rest.Data.Ef.Context;

namespace Radilovsoft.Rest.Data.Ef.Provider
{
    public class EfDataProvider : IDataProvider
    {
        private readonly ResetDbContext _dbContext;
        private readonly IDataExceptionManager _exceptionManager;

        public EfDataProvider(
            ResetDbContext connection,
            IDataExceptionManager dataExceptionManager)
        {
            _dbContext = connection ?? throw new ArgumentNullException(nameof(connection));
            _exceptionManager = dataExceptionManager ?? throw new ArgumentNullException(nameof(dataExceptionManager));
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

        public IQueryable<T> GetQueryable<T>() where T : class
        {
            return _dbContext.Set<T>().AsQueryable().AsNoTracking();
        }

        #region Modify

        public Task<T> InsertAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            return ExecuteCommand(async state =>
            {
                Add(state.entity);
                await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
                return state.entity;
            }, (entity, token: cancellationToken));
        }

        public Task<T> UpdateAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            return ExecuteCommand(async state =>
            {
                UpdateEntity(state.entity);
                await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
                return state.entity;
            }, (entity, token: cancellationToken));
        }

        public Task DeleteAsync<T>(T entity, CancellationToken cancellationToken = default) where T : class
        {
            return ExecuteCommand(state =>
            {
                _dbContext.Set<T>().Remove(state.entity);
                return _dbContext.SaveChangesAsync(state.token);
            }, (entity, token: cancellationToken));
        }

        public Task DeleteByIdAsync<T, TKey>(TKey id, CancellationToken cancellationToken = default)
            where T : class, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entity = await _dbContext.Set<T>().Where(t => state.id.Equals(t.Id)).SingleAsync(state.token)
                    .ConfigureAwait(false);
                _dbContext.Set<T>().Remove(entity);
                return await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
            }, (id, token: cancellationToken));
        }

        public Task SetDeleteAsync<T, TKey>(TKey id, CancellationToken cancellationToken = default)
            where T : class, IDeletable, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entity = await _dbContext.Set<T>().Where(t => state.id.Equals(t.Id)).SingleAsync(state.token)
                    .ConfigureAwait(false);
                entity.DeletedUtc = DateTime.UtcNow;
                UpdateEntity(entity);
                return await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
            }, (id, token: cancellationToken));
        }

        public Task SetUnDeleteAsync<T, TKey>(TKey id, CancellationToken cancellationToken = default)
            where T : class, IDeletable, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entity = await _dbContext.Set<T>().Where(t => state.id.Equals(t.Id)).SingleAsync(state.token)
                    .ConfigureAwait(false);
                entity.DeletedUtc = null;
                UpdateEntity(entity);
                return await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
            }, (id, token: cancellationToken));
        }

        #endregion

        #region BatchModify

        public Task BatchInsertAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            where T : class
        {
            return ExecuteCommand(state =>
            {
                foreach (var entity in state.entities)
                {
                    Add(entity);
                }

                return _dbContext.SaveChangesAsync(state.token);
            }, (entities, token: cancellationToken));
        }

        public Task BatchUpdateAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            where T : class
        {
            return ExecuteCommand(state =>
            {
                foreach (var entity in state.entities)
                {
                    UpdateEntity(entity);
                }

                return _dbContext.SaveChangesAsync(state.token);
            }, (entities, token: cancellationToken));
        }

        public Task BatchDeleteAsync<T>(IEnumerable<T> entities, CancellationToken cancellationToken = default)
            where T : class
        {
            return ExecuteCommand(state =>
            {
                _dbContext.Set<T>().RemoveRange(state.entities);
                return _dbContext.SaveChangesAsync(state.token);
            }, (entities, token: cancellationToken));
        }

        public Task BatchDeleteByIdsAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
            where T : class, IEntity<TKey>
            where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entity = await _dbContext.Set<T>().Where(t => state.ids.Contains(t.Id)).ToArrayAsync(state.token)
                    .ConfigureAwait(false);
                _dbContext.Set<T>().RemoveRange(entity);
                return await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
            }, (ids, token: cancellationToken));
        }

        public Task BatchSetDeleteAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
            where T : class, IDeletable, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entities = await _dbContext.Set<T>().Where(t => state.ids.Contains(t.Id)).ToArrayAsync(state.token)
                    .ConfigureAwait(false);

                foreach (var entity in entities)
                {
                    entity.DeletedUtc = DateTime.UtcNow;
                    UpdateEntity(entity);
                }

                return await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
            }, (ids, token: cancellationToken));
        }

        public Task BatchSetUnDeleteAsync<T, TKey>(IEnumerable<TKey> ids, CancellationToken cancellationToken = default)
            where T : class, IDeletable, IEntity<TKey> where TKey : IComparable
        {
            return ExecuteCommand(async state =>
            {
                var entities = await _dbContext.Set<T>().Where(t => state.ids.Contains(t.Id)).ToArrayAsync(state.token)
                    .ConfigureAwait(false);

                foreach (var entity in entities)
                {
                    entity.DeletedUtc = null;
                    UpdateEntity(entity);
                }

                return await _dbContext.SaveChangesAsync(state.token).ConfigureAwait(false);
            }, (ids, token: cancellationToken));
        }

        #endregion

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

        private void UpdateEntity<T>(T entity) where T : class
        {
            if (entity is IUpdatedUtc updatedUtc)
            {
                updatedUtc.UpdatedUtc = DateTime.UtcNow;
            }

            var entityEntry = _dbContext.Entry(entity);
            entityEntry.State = EntityState.Modified;
        }

        private Task<T> ExecuteCommand<T, TState>(Func<TState, Task<T>> func, TState state)
        {
            try
            {
                return func(state);
            }
            catch (Exception exception)
            {
                throw _exceptionManager.Normalize(exception);
            }
        }
    }
}
using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;
using REST.DataCore.Provider;

namespace REST.EfCore.Provider
{
    internal class EfDataTransactionAdapter : IDataTransaction
    {
        private readonly IDbContextTransaction _transaction;

        public EfDataTransactionAdapter([NotNull] IDbContextTransaction transaction)
        {
            _transaction = transaction ?? throw new ArgumentNullException(nameof(transaction));
        }

        public void Commit()
        {
            _transaction.Commit();
        }

        public void Rollback()
        {
            _transaction.Rollback();
        }

        public Task CommitAsync(CancellationToken cancellationToken = default)
        {
            return _transaction.CommitAsync(cancellationToken);
        }

        public Task RollbackAsync(CancellationToken cancellationToken = default)
        {
            return _transaction.RollbackAsync(cancellationToken);
        }

        public void Dispose()
        {
            _transaction.Dispose();
        }

        public ValueTask DisposeAsync()
        {
            return _transaction.DisposeAsync();
        }

        public object GetTransaction()
        {
            return _transaction;
        }
    }
}
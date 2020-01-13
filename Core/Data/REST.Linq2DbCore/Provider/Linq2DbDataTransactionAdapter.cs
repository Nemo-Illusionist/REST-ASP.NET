using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using LinqToDB.Data;
using REST.DataCore.Provider;

namespace REST.Linq2DbCore.Provider
{
    internal class Linq2DbDataTransactionAdapter : IDataTransaction
    {
        private readonly DataConnectionTransaction _transaction;

        public Linq2DbDataTransactionAdapter([NotNull] DataConnectionTransaction transaction)
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
            return new ValueTask(_transaction.DataConnection.RollbackTransactionAsync());
        }

        public object GetTransaction()
        {
            return _transaction;
        }
    }
}
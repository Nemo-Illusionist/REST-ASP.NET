using System;
using JetBrains.Annotations;
using LinqToDB.Data;
using System.Threading;
using System.Threading.Tasks;
using REST.DataCore.Provider;

namespace Linq2DbCodre.Provider
{
    internal class DataTransactionAdapter : IDataTransaction
    {
        private readonly DataConnectionTransaction _transaction;

        public DataTransactionAdapter([NotNull] DataConnectionTransaction transaction)
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
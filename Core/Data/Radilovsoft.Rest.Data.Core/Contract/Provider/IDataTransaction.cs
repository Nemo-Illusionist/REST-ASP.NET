using System;
using System.Threading;
using System.Threading.Tasks;

namespace Radilovsoft.Rest.Data.Core.Contract.Provider
{
    public interface IDataTransaction : IDisposable, IAsyncDisposable
    {
        void Commit();
        void Rollback();

        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
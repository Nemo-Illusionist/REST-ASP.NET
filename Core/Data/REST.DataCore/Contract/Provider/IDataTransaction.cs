using System;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace REST.DataCore.Contract.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IDataTransaction : IDisposable, IAsyncDisposable
    {
        void Commit();
        void Rollback();

        Task CommitAsync(CancellationToken cancellationToken = default);
        Task RollbackAsync(CancellationToken cancellationToken = default);
    }
}
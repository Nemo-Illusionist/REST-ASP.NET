using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace Radilovsoft.Rest.Data.Core.Contract.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface ISafeExecuteProvider
    {
        Task<T> SafeExecuteAsync<T>(
            [InstantHandle] Func<IDataProvider, CancellationToken, Task<T>> action,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default);

        Task SafeExecuteAsync(
            [InstantHandle] Func<IDataProvider, CancellationToken, Task> action,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default);
    }
}
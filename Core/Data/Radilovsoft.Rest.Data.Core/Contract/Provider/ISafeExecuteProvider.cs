using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Radilovsoft.Rest.Data.Core.Contract.Provider
{
    public interface ISafeExecuteProvider
    {
        Task<T> SafeExecuteAsync<T>(
            IDataProvider provider,
            Func<IDataProvider, CancellationToken, Task<T>> func,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken cancellationToken = default);

        Task SafeExecuteAsync
        (IDataProvider provider,
            Func<IDataProvider, CancellationToken, Task> func,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken cancellationToken = default);
    }
}
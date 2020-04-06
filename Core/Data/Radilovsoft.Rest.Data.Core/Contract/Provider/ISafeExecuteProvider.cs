using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;

namespace Radilovsoft.Rest.Data.Core.Contract.Provider
{
    public interface ISafeExecuteProvider
    {
        Task<T> SafeExecuteAsync<T>(
            Func<IDataProvider, CancellationToken, Task<T>> func,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default);

        Task SafeExecuteAsync(
            Func<IDataProvider, CancellationToken, Task> func,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default);

        Task<T> SafeExecuteAsync<T>(
            Func<IDataProvider, CancellationToken, Task<T>> func,
            IDataProvider provider,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default);

        Task SafeExecuteAsync(
            Func<IDataProvider, CancellationToken, Task> func,
            IDataProvider provider,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default);
    }
}
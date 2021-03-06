using System;
using System.Data;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Radilovsoft.Rest.Data.Core.Contract;
using Radilovsoft.Rest.Data.Core.Contract.Provider;

namespace Radilovsoft.Rest.Data.Core.Provider
{
    public class SafeExecuteProvider : ISafeExecuteProvider
    {
        protected IDataExceptionManager ExceptionManager { get; }

        protected SafeExecuteProvider(IDataExceptionManager dataExceptionManager)
        {
            ExceptionManager = dataExceptionManager
                               ?? throw new ArgumentNullException(nameof(dataExceptionManager));
        }

        public Task<T> SafeExecuteAsync<T>(
            Func<IDataProvider, CancellationToken, Task<T>> func,
            IDataProvider provider,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default)
        {
            return SafeExecuteAsync(WrapperTaskT, func, provider, level, retryCount, token);
        }

        public Task SafeExecuteAsync(
            Func<IDataProvider, CancellationToken, Task> func,
            IDataProvider provider,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default)
        {
            return SafeExecuteAsync(WrapperTask<VoidStruct>, func, provider, level, retryCount, token);
        }

        private async Task<T> SafeExecuteAsync<T, TArg>(
            Func<IDataProvider, TArg, CancellationToken, Task<T>> func,
            TArg arg,
            IDataProvider provider,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default)
        {
            T result;
            var count = 0;
            while (true)
            {
                try
                {
                    await using var transaction = provider.Transaction(level);
                    result = await func(provider, arg, token).ConfigureAwait(false);
                    await transaction.CommitAsync(token).ConfigureAwait(false);
                    break;
                }
                catch (Exception exception)
                {
                    Reset();

                    if (ExceptionManager.IsRepeatAction(exception) && ++count >= retryCount) throw;

                    await Task.Delay(TimeSpan.FromSeconds(1), token).ConfigureAwait(false);
                }
            }

            return result;
        }

        protected virtual void Reset()
        {
        }

        [StructLayout(LayoutKind.Auto)]
        private readonly struct VoidStruct
        {
        }

        private static async Task<T> WrapperTask<T>(
            IDataProvider dp,
            Func<IDataProvider, CancellationToken, Task> f,
            CancellationToken t)
        {
            await f(dp, t);
            return default;
        }

        private static Task<T> WrapperTaskT<T>(
            IDataProvider dp,
            Func<IDataProvider, CancellationToken, Task<T>> f,
            CancellationToken t)
        {
            return f(dp, t);
        }
    }
}
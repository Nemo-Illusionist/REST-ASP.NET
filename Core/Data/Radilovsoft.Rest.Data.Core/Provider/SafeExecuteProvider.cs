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
            ExceptionManager = dataExceptionManager ?? throw new ArgumentNullException(nameof(dataExceptionManager));
        }

        public Task<T> SafeExecuteAsync<T>(
            IDataProvider provider,
            Func<IDataProvider, CancellationToken, Task<T>> func,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken cancellationToken = default)
        {
            return SafeExecuteAsync(provider, WrapperTaskT, func, level, retryCount, cancellationToken);
        }

        public Task SafeExecuteAsync(
            IDataProvider provider,
            Func<IDataProvider, CancellationToken, Task> func,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken cancellationToken = default)
        {
            return SafeExecuteAsync(provider, WrapperTask<VoidStruct>, func, level, retryCount, cancellationToken);
        }

        private async Task<T> SafeExecuteAsync<T, TArg>(
            IDataProvider provider,
            Func<IDataProvider, TArg, CancellationToken, Task<T>> func,
            TArg arg,
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
                catch (Exception exception) when (ExceptionManager.IsRepeatAction(exception) && ++count < retryCount)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), token).ConfigureAwait(false);
                }
                finally
                {
                    Reset();
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
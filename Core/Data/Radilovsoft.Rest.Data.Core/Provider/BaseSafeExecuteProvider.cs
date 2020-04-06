using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Radilovsoft.Rest.Data.Core.Contract;
using Radilovsoft.Rest.Data.Core.Contract.Provider;

namespace Radilovsoft.Rest.Data.Core.Provider
{
    public abstract class BaseSafeExecuteProvider : ISafeExecuteProvider
    {
        protected IDataExceptionManager ExceptionManager { get; }

        protected BaseSafeExecuteProvider(IDataExceptionManager dataExceptionManager)
        {
            ExceptionManager = dataExceptionManager
                               ?? throw new ArgumentNullException(nameof(dataExceptionManager));
        }

        public Task<T> SafeExecuteAsync<T>(
            Func<IDataProvider, CancellationToken, Task<T>> func,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default)
        {
            return SafeExecuteAsync(func, GetProvider(), level, retryCount, token);
        }


        public Task SafeExecuteAsync(
            Func<IDataProvider, CancellationToken, Task> func,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default)
        {
            return SafeExecuteAsync(func, GetProvider(), level, retryCount, token);
        }

        public async Task<T> SafeExecuteAsync<T>(
            Func<IDataProvider, CancellationToken, Task<T>> func,
            IDataProvider provider,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default)
        {
            var result = default(T);
            await SafeExecuteAsync(Wrapper, provider, level, retryCount, token).ConfigureAwait(false);
            return result;

            async Task Wrapper(IDataProvider db, CancellationToken ct) =>
                result = await func(db, ct).ConfigureAwait(false);
        }

        public async Task SafeExecuteAsync(
            Func<IDataProvider, CancellationToken, Task> func,
            IDataProvider provider,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default)
        {
            var count = 0;
            while (true)
            {
                try
                {
                    await using var transaction = provider.Transaction(level);
                    await func(provider, token).ConfigureAwait(false);
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
        }

        protected abstract void Reset();
        protected abstract IDataProvider GetProvider();
    }
}
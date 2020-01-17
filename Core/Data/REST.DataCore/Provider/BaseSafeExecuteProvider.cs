using System;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using JetBrains.Annotations;
using REST.DataCore.Contract;
using REST.DataCore.Contract.Provider;

namespace REST.DataCore.Provider
{
    public abstract class BaseSafeExecuteProvider : ISafeExecuteProvider
    {
        protected IDataExceptionManager ExceptionManager { get; }

        protected BaseSafeExecuteProvider([NotNull] IDataExceptionManager dataExceptionManager)
        {
            ExceptionManager = dataExceptionManager
                               ?? throw new ArgumentNullException(nameof(dataExceptionManager));
        }

        public async Task<T> SafeExecuteAsync<T>(
            [InstantHandle] Func<IDataProvider, CancellationToken, Task<T>> action,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default)
        {
            var result = default(T);
            await SafeExecuteAsync(Wrapper, level, retryCount, token).ConfigureAwait(false);
            return result;

            async Task Wrapper(IDataProvider db, CancellationToken ct) =>
                result = await action(db, ct).ConfigureAwait(false);
        }


        public async Task SafeExecuteAsync(
            [InstantHandle] Func<IDataProvider, CancellationToken, Task> action,
            IsolationLevel level = IsolationLevel.RepeatableRead,
            int retryCount = 3,
            CancellationToken token = default)
        {
            var count = 0;
            while (true)
            {
                try
                {
                    var provider = GetProvider();
                    await using var transaction = provider.Transaction(level);
                    await action(provider, token).ConfigureAwait(false);
                    await transaction.CommitAsync(token).ConfigureAwait(false);
                    break;
                }
                catch (Exception exception)
                {
                    Reset();

                    if (ExceptionManager.IsConcurrentModifyException(exception) && ++count >= retryCount) throw;

                    await Task.Delay(TimeSpan.FromSeconds(1), token).ConfigureAwait(false);
                }
            }
        }

        protected abstract void Reset();
        protected abstract IDataProvider GetProvider();
    }
}
using System;
using System.Data;
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
        
        protected virtual void Reset()
        {
        }
    }
}
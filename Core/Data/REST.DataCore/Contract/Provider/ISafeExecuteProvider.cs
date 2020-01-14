using System;
using System.Data;
using System.Threading.Tasks;
using JetBrains.Annotations;

namespace REST.DataCore.Contract.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface ISafeExecuteProvider
    {
        Task<T> SafeExecuteAsync<T>([InstantHandle] Func<IDataProvider, Task<T>> action,
            IsolationLevel level = IsolationLevel.RepeatableRead, int retryCount = 3);

        Task SafeExecuteAsync([InstantHandle] Func<IDataProvider, Task> action,
            IsolationLevel level = IsolationLevel.RepeatableRead, int retryCount = 3);
    }
}
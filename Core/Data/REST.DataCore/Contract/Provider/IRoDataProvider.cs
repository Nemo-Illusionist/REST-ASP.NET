using System.Data;
using System.Linq;
using JetBrains.Annotations;
using REST.DataCore.Contract.Entity;

namespace REST.DataCore.Contract.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IRoDataProvider
    {
        IDataTransaction Transaction();
        IDataTransaction Transaction(IsolationLevel isolationLevel);

        IQueryable<T> GetQueryable<T>() where T : class, IEntity;
    }
}
using System.Data;
using System.Linq;
using JetBrains.Annotations;
using REST.DataCore.EntityContract;

namespace REST.DataCore.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IRoDataProvider
    {
        IDataTransaction Transaction();
        IDataTransaction Transaction(IsolationLevel isolationLevel);

        IQueryable<T> GetQueryable<T>() where T : class, IEntity;
    }
}
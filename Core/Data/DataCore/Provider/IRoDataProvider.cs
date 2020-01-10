using System.Data;
using System.Linq;
using DataCore.EntityContract;
using JetBrains.Annotations;

namespace DataCore.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IRoDataProvider
    {
        IDataTransaction Transaction();
        IDataTransaction Transaction(IsolationLevel isolationLevel);

        IQueryable<T> GetQueryable<T>() where T : class, IEntity;
    }
}
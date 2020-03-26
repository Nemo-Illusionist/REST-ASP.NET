using System.Data;
using System.Linq;
using JetBrains.Annotations;
using Radilovsoft.Rest.Data.Core.Contract.Entity;

namespace Radilovsoft.Rest.Data.Core.Contract.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IRoDataProvider
    {
        IDataTransaction Transaction();
        IDataTransaction Transaction(IsolationLevel isolationLevel);

        IQueryable<T> GetQueryable<T>() where T : class, IEntity;
    }
}
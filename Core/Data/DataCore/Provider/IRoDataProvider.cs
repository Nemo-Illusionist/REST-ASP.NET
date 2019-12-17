using System.Linq;
using DataCore.EntityContract;
using JetBrains.Annotations;

namespace DataCore.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IRoDataProvider
    {
        IQueryable<T> GetQueryable<T>() where T : class, IEntity;
    }
}
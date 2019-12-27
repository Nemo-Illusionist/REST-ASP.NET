using JetBrains.Annotations;

namespace DataCore.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IDataProvider : IRoDataProvider, IWoDataProvider, IDeleteDataProvider
    {

    }
}
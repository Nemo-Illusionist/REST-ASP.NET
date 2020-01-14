using JetBrains.Annotations;

namespace REST.DataCore.Contract.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IDataProvider : IRwDataProvider, IDeleteDataProvider
    {
    }
}
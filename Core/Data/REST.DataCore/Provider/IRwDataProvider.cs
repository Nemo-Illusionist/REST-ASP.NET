using JetBrains.Annotations;

namespace REST.DataCore.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IRwDataProvider : IRoDataProvider, IWoDataProvider
    {
    }
}
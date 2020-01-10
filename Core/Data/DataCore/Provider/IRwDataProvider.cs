using JetBrains.Annotations;

namespace DataCore.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IRwDataProvider : IRoDataProvider, IWoDataProvider
    {
    }
}
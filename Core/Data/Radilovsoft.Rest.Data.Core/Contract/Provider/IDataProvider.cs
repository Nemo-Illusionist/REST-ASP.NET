using JetBrains.Annotations;

namespace Radilovsoft.Rest.Data.Core.Contract.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface IDataProvider : IRwDataProvider, IDeleteDataProvider
    {
    }
}
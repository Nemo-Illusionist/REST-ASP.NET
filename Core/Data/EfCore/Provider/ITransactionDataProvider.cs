using System.Data;
using DataCore.Provider;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore.Storage;

namespace EfCore.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface ITransactionDataProvider : IDataProvider
    {
        IDbContextTransaction Transaction();
        IDbContextTransaction Transaction(IsolationLevel isolationLevel);
    }
}
using DataCore.Provider;
using JetBrains.Annotations;
using LinqToDB.Data;
using System.Data;

namespace Linq2DbCodre.Provider
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public interface ITransactionDataProvider : IDataProvider
    {
        DataConnectionTransaction Transaction();
        DataConnectionTransaction Transaction(IsolationLevel isolationLevel);
    }
}
using System.Data;
using System.Linq;

namespace Radilovsoft.Rest.Data.Core.Contract.Provider
{
    public interface IRoDataProvider
    {
        IDataTransaction Transaction();
        IDataTransaction Transaction(IsolationLevel isolationLevel);

        IQueryable<T> GetQueryable<T>() where T : class;
    }
}
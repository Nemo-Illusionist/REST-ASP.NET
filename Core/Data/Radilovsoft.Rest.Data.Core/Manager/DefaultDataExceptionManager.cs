using System;
using Radilovsoft.Rest.Data.Core.Contract;

namespace Radilovsoft.Rest.Data.Core.Manager
{
    public class DefaultDataExceptionManager : IDataExceptionManager
    {
        public Exception Normalize(Exception ex)
        {
            return ex;
        }

        public bool IsConcurrentModifyException(Exception ex)
        {
            return true;
        }
    }
}
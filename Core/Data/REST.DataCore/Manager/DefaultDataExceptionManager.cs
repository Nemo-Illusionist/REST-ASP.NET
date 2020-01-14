using System;
using REST.DataCore.Contract;

namespace REST.DataCore.Manager
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
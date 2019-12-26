using System;

namespace DataCore.Manager
{
    public class DefaultDbExceptionManager : IDbExceptionManager
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
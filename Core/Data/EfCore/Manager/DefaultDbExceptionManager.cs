using System;

namespace EfCore.Manager
{
    internal class DefaultDbExceptionManager : IDbExceptionManager
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
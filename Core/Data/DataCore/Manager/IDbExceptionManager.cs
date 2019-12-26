using System;

namespace DataCore.Manager
{
    public interface IDbExceptionManager
    {
        Exception Normalize(Exception ex);
        bool IsConcurrentModifyException(Exception ex);
    }
}
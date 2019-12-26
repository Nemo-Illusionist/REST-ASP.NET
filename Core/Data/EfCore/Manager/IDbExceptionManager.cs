using System;

namespace EfCore.Manager
{
    public interface IDbExceptionManager
    {
        Exception Normalize(Exception ex);
        bool IsConcurrentModifyException(Exception ex);
    }
}
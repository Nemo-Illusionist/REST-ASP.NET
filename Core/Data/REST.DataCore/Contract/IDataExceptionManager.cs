using System;

namespace REST.DataCore.Contract
{
    public interface IDataExceptionManager
    {
        Exception Normalize(Exception ex);
        bool IsConcurrentModifyException(Exception ex);
    }
}
using System;

namespace Radilovsoft.Rest.Data.Core.Contract
{
    public interface IDataExceptionManager
    {
        Exception Normalize(Exception ex);
        bool IsRepeatAction(Exception ex);
    }
}
using System;
using JetBrains.Annotations;
using Npgsql;
using Radilovsoft.Rest.Data.Core.Contract;
using Radilovsoft.Rest.Data.Core.Exceptions;

namespace Radilovsoft.Rest.Data.Postgres
{
    public class PostgresDbExceptionManager : IDataExceptionManager
    {
        public Exception Normalize([NotNull] Exception exception)
        {
            if (exception == null) throw new ArgumentNullException(nameof(exception));
            
            if (exception.InnerException is PostgresException ex)
            {
                var message = ex.Message + ex.Detail;

                switch (ex.SqlState)
                {
                    case PostgresErrorCodes.ForeignKeyViolation:
                        return new ForeignKeyViolationException(message, ex);
                    case PostgresErrorCodes.UniqueViolation:
                        return new ObjectAlreadyExistsException(message, ex);
                    case PostgresErrorCodes.SerializationFailure:
                        return new ConcurrentModifyException(message, ex);
                    default:
                        return ex;
                }
            }

            return exception;
        }

        public bool IsRepeatAction(Exception ex)
        {
            return ex is ConcurrentModifyException;
        }
    }

}
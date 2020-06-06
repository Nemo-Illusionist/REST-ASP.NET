namespace Radilovsoft.Rest.Data.Core.Exceptions
{
    public class ForeignKeyViolationException : System.Exception
    {
        public ForeignKeyViolationException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        public ForeignKeyViolationException(string message) : base(message)
        {
        }

        public ForeignKeyViolationException()
        {
        }
    }
}
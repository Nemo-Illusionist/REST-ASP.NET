namespace Radilovsoft.Rest.Data.Core.Exceptions
{
    public class ConcurrentModifyException : System.Exception
    {
        public ConcurrentModifyException()
        {
        }

        public ConcurrentModifyException(string message) : base(message)
        {
        }

        public ConcurrentModifyException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
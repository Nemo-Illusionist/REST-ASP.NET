namespace Radilovsoft.Rest.Data.Core.Exceptions
{
    public class ObjectAlreadyExistsException : System.Exception
    {
        public ObjectAlreadyExistsException()
        {
        }

        public ObjectAlreadyExistsException(string message) : base(message)
        {
        }

        public ObjectAlreadyExistsException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
namespace Radilovsoft.Rest.Data.Core.Exceptions
{
    public class ObjectAlreadyExistsException : System.Exception
    {
        public ObjectAlreadyExistsException(string message) : base(message)
        {
        }

        public ObjectAlreadyExistsException()
        {
        }

        public ObjectAlreadyExistsException(string message, System.Exception innerException) : base(message, innerException)
        {
        }
    }
}
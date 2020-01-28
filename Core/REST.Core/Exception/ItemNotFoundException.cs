namespace REST.Core.Exception
{
    public class ItemNotFoundException:System.Exception
    {
        public ItemNotFoundException(string message, System.Exception innerException) : base(message, innerException)
        {
        }

        public ItemNotFoundException(string message) : base(message)
        {
        }

        public ItemNotFoundException()
        {
        }
    }
}
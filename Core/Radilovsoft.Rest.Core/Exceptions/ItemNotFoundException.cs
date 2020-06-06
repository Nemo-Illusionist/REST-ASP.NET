using System;

namespace Radilovsoft.Rest.Core.Exceptions
{
    public class ItemNotFoundException : Exception
    {
        public ItemNotFoundException(string message, Exception innerException) : base(message, innerException)
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
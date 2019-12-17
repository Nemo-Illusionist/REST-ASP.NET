using System;

namespace EfCore.Manager
{
    internal class DefaultNormalizeException : INormalizeException
    {
        public Exception Normalize(Exception ex)
        {
            return ex;
        }
    }
}
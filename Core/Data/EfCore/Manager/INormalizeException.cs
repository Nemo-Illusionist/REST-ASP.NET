
using System;

namespace EfCore.Manager
{
    public interface INormalizeException
    {
        public Exception Normalize(Exception ex);
    }
}
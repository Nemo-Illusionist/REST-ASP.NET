using System;
using System.Collections.Generic;

namespace REST.EfCore.Store
{
    public interface IModelStore
    {
        IEnumerable<Type> GetModels();
    }
}
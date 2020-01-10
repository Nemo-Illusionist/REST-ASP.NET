using System;
using System.Collections.Generic;

namespace REST.DataCore.Store
{
    public interface IModelStore
    {
        IEnumerable<Type> GetModels();
    }
}
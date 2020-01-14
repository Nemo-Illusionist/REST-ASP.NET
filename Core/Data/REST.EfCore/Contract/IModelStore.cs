using System;
using System.Collections.Generic;

namespace REST.EfCore.Contract
{
    public interface IModelStore
    {
        IEnumerable<Type> GetModels();
    }
}
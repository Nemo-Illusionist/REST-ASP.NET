using System;
using System.Collections.Generic;

namespace DataCore.Store
{
    public interface IModelStore
    {
        IEnumerable<Type> GetModels();
    }
}
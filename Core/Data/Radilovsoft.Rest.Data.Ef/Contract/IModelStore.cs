using System;
using System.Collections.Generic;

namespace Radilovsoft.Rest.Data.Ef.Contract
{
    public interface IModelStore
    {
        IEnumerable<Type> GetModels();
    }
}
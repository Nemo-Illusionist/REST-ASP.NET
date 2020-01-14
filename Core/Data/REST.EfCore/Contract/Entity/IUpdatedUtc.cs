using System;
using REST.EfCore.Annotation;

namespace REST.EfCore.Contract.Entity
{
    public interface IUpdatedUtc : DataCore.Contract.Entity.IUpdatedUtc
    {
        [Index]
        new DateTime UpdatedUtc { get; set; }
    }
}
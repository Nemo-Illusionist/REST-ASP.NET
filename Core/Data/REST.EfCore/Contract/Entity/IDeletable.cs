using System;
using REST.EfCore.Annotation;

namespace REST.EfCore.Contract.Entity
{
    public interface IDeletable : DataCore.Contract.Entity.IDeletable
    {
        [Index]
        new DateTime? DeletedUtc { get; set; }
    }
}
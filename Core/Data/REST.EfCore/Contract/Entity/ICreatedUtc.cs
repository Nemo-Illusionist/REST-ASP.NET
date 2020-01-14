using System;
using REST.EfCore.Annotation;

namespace REST.EfCore.Contract.Entity
{
    public interface ICreatedUtc : DataCore.Contract.Entity.ICreatedUtc
    {
        [Index]
        new DateTime CreatedUtc { get; set; }
    }
}
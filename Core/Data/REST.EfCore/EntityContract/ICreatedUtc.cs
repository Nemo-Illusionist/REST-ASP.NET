using System;
using REST.EfCore.Annotation;

namespace REST.EfCore.EntityContract
{
    public interface ICreatedUtc : REST.DataCore.EntityContract.ICreatedUtc
    {
        [Index]
        new DateTime CreatedUtc { get; set; }
    }
}
using System;
using REST.EfCore.Annotation;

namespace REST.EfCore.EntityContract
{
    public interface IDeletable : REST.DataCore.EntityContract.IDeletable
    {
        [Index]
        new DateTime? DeletedUtc { get; set; }
    }
}
using System;
using EfCore.Annotation;

namespace EfCore.EntityContract
{
    public interface IDeletable : REST.DataCore.EntityContract.IDeletable
    {
        [Index]
        new DateTime? DeletedUtc { get; set; }
    }
}
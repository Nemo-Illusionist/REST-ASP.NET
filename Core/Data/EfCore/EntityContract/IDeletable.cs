using System;
using EfCore.Annotation;

namespace EfCore.EntityContract
{
    public interface IDeletable : DataCore.EntityContract.IDeletable
    {
        [Index]
        new DateTime? DeletedUtc { get; set; }
    }
}
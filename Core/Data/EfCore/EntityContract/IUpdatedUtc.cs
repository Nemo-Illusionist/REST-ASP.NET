using System;
using EfCore.Annotation;

namespace EfCore.EntityContract
{
    public interface IUpdatedUtc : DataCore.EntityContract.IUpdatedUtc
    {
        [Index]
        new DateTime UpdatedUtc { get; set; }
    }
}
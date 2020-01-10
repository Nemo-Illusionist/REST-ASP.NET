using System;
using EfCore.Annotation;

namespace EfCore.EntityContract
{
    public interface IUpdatedUtc : REST.DataCore.EntityContract.IUpdatedUtc
    {
        [Index]
        new DateTime UpdatedUtc { get; set; }
    }
}
using System;
using REST.EfCore.Annotation;

namespace REST.EfCore.EntityContract
{
    public interface IUpdatedUtc : REST.DataCore.EntityContract.IUpdatedUtc
    {
        [Index]
        new DateTime UpdatedUtc { get; set; }
    }
}
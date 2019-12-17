using System;
using DataCore.Annotation;

namespace DataCore.EntityContract
{
    public interface IUpdatedUtc : IEntity
    {
        [Index]
        DateTime UpdatedUtc { get; set; }
    }
}
using System;
using DataCore.Annotation;

namespace DataCore.EntityContract
{
    public interface IDeletable : IEntity
    {
        [Index]
        DateTime? DeletedUtc { get; set; }
    }
}
using System;
using DataCore.Annotation;

namespace DataCore.EntityContract
{
    public interface ICreatedUtc : IEntity
    {
        [Index]
        DateTime CreatedUtc { get; set; }
    }
}
using System;
using EfCore.Annotation;

namespace EfCore.EntityContract
{
    public interface ICreatedUtc : DataCore.EntityContract.ICreatedUtc
    {
        [Index]
        new DateTime CreatedUtc { get; set; }
    }
}
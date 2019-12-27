using System;

namespace DataCore.EntityContract
{
    public interface ICreatedUtc : IEntity
    {
        DateTime CreatedUtc { get; set; }
    }
}
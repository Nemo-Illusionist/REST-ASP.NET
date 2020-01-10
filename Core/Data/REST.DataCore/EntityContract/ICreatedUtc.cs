using System;

namespace REST.DataCore.EntityContract
{
    public interface ICreatedUtc : IEntity
    {
        DateTime CreatedUtc { get; set; }
    }
}
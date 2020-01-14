using System;

namespace REST.DataCore.Contract.Entity
{
    public interface ICreatedUtc : IEntity
    {
        DateTime CreatedUtc { get; set; }
    }
}
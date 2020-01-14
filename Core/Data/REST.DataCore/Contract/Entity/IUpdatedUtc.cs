using System;

namespace REST.DataCore.Contract.Entity
{
    public interface IUpdatedUtc : IEntity
    {
        DateTime UpdatedUtc { get; set; }
    }
}
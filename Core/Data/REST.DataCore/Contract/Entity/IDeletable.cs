using System;

namespace REST.DataCore.Contract.Entity
{
    public interface IDeletable : IEntity
    {
        DateTime? DeletedUtc { get; set; }
    }
}
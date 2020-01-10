using System;

namespace REST.DataCore.EntityContract
{
    public interface IDeletable : IEntity
    {
        DateTime? DeletedUtc { get; set; }
    }
}
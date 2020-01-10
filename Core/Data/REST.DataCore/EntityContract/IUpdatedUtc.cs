using System;

namespace REST.DataCore.EntityContract
{
    public interface IUpdatedUtc : IEntity
    {
        DateTime UpdatedUtc { get; set; }
    }
}
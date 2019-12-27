using System;

namespace DataCore.EntityContract
{
    public interface IUpdatedUtc : IEntity
    {
        DateTime UpdatedUtc { get; set; }
    }
}
using System;

namespace DataCore.EntityContract
{
    public interface IDeletable : IEntity
    {
        DateTime? DeletedUtc { get; set; }
    }
}
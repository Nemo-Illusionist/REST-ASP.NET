using System;

namespace Radilovsoft.Rest.Data.Core.Contract.Entity
{
    public interface IDeletable : IEntity
    {
        DateTime? DeletedUtc { get; set; }
    }
}
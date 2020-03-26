using System;

namespace Radilovsoft.Rest.Data.Core.Contract.Entity
{
    public interface IUpdatedUtc : IEntity
    {
        DateTime UpdatedUtc { get; set; }
    }
}
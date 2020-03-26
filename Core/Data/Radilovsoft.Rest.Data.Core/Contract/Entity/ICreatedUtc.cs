using System;

namespace Radilovsoft.Rest.Data.Core.Contract.Entity
{
    public interface ICreatedUtc : IEntity
    {
        DateTime CreatedUtc { get; set; }
    }
}
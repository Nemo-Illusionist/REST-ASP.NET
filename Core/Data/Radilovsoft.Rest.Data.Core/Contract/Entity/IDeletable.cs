using System;

namespace Radilovsoft.Rest.Data.Core.Contract.Entity
{
    public interface IDeletable
    {
        DateTime? DeletedUtc { get; set; }
    }
}
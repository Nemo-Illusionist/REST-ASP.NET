using System;
using System.Diagnostics.CodeAnalysis;

namespace Radilovsoft.Rest.Data.Core.Contract.Entity
{
    [SuppressMessage("ReSharper", "CA1040")]
    public interface IEntity
    {
    }

    public interface IEntity<out TKey> : IEntity where TKey : IComparable
    {
        TKey Id { get; }
    }
}
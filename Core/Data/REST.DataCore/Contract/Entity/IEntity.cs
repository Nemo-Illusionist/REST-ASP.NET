using System;
using System.Diagnostics.CodeAnalysis;

namespace REST.DataCore.Contract.Entity
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
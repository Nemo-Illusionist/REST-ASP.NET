using System;
using System.Diagnostics.CodeAnalysis;

namespace REST.DataCore.EntityContract
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
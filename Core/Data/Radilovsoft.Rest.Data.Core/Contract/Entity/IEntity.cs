using System;

namespace Radilovsoft.Rest.Data.Core.Contract.Entity
{
    public interface IEntity<out TKey> where TKey : IComparable
    {
        TKey Id { get; }
    }
}
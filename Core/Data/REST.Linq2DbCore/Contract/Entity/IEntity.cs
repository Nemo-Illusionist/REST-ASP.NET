using System;
using LinqToDB.Mapping;

namespace REST.Linq2DbCore.Contract.Entity
{
    public interface IEntity<out TKey> : DataCore.Contract.Entity.IEntity<TKey> where TKey : IComparable
    {
        [PrimaryKey]
        new TKey Id { get; }
    }
}
using System;
using LinqToDB.Mapping;

namespace REST.Linq2DbCodre.EntityContract
{
    public interface IEntity<out TKey> : REST.DataCore.EntityContract.IEntity<TKey> where TKey : IComparable
    {
        [PrimaryKey]
        new TKey Id { get; }
    }
}
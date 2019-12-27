using System;
using LinqToDB.Mapping;

namespace Linq2DbCodre.EntityContract
{
    public interface IEntity<out TKey> : DataCore.EntityContract.IEntity<TKey> where TKey : IComparable
    {
        [PrimaryKey]
        new TKey Id { get; }
    }
}
using System;

namespace DataCore.EntityContract
{
    public interface IEntity
    {
    }
    
    public interface IEntity<out TKey> where TKey : IComparable
    {
        TKey Id { get; }
    }
}
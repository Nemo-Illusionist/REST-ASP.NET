using System;
using System.ComponentModel.DataAnnotations;

namespace REST.EfCore.EntityContract
{
    public interface IEntity<out TKey> : REST.DataCore.EntityContract.IEntity<TKey> where TKey : IComparable
    {
        [Key] 
        new TKey Id { get; }
    }
}
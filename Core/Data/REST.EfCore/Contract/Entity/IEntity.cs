using System;
using System.ComponentModel.DataAnnotations;

namespace REST.EfCore.Contract.Entity
{
    public interface IEntity<out TKey> : DataCore.Contract.Entity.IEntity<TKey> where TKey : IComparable
    {
        [Key] 
        new TKey Id { get; }
    }
}
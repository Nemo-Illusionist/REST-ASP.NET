using System;
using System.Collections.Generic;

namespace Radilovsoft.Rest.Core.Comparer
{
    public class NullUniqueEqualityComparer<T> : IEqualityComparer<T>
    {
        private NullUniqueEqualityComparer()
        {
        }

        private static readonly Lazy<NullUniqueEqualityComparer<T>> LazyInstance =
            new Lazy<NullUniqueEqualityComparer<T>>(() => new NullUniqueEqualityComparer<T>());

        public static NullUniqueEqualityComparer<T> Instance => LazyInstance.Value;

        
        public bool Equals(T x, T y)
        {
            if (x == null || y == null) return false;
            return x.Equals(y);
        }

        public int GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
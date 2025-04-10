using System;
using System.Collections.Generic;

namespace Mathlife.ProjectL.Utils
{
    public class LambdaEqualityComparer<T> : IEqualityComparer<T> 
    {
        protected readonly Func<T, T, bool> equals;
        protected readonly Func<T, int> getHashCode;
        
        protected static readonly Func<T, int> defaultGetHashCode = t => t.GetHashCode();
        
        public static LambdaEqualityComparer<T> Create(Func<T, T, bool> equals, Func<T, int> getHashCode = null)
        {
            return new LambdaEqualityComparer<T>(equals, getHashCode);
        }
        
        protected LambdaEqualityComparer(Func<T, T, bool> equals, Func<T, int> getHashCode)
        {
            this.equals = equals ?? throw new ArgumentNullException(nameof(equals));
            this.getHashCode = getHashCode ?? defaultGetHashCode;
        }
        
        public bool Equals(T x, T y)
        {
            return equals(x, y);
        }

        public int GetHashCode(T obj)
        {
            return getHashCode(obj);
        }
    }
}
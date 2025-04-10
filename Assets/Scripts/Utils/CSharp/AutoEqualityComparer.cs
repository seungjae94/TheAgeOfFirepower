using System;

namespace Mathlife.ProjectL.Utils
{
    public class AutoEqualityComparer<T> : LambdaEqualityComparer<T> where T : IEquatable<T>
    {
        public static AutoEqualityComparer<T> Create()
        {
            return new AutoEqualityComparer<T>();
        }
        
        protected AutoEqualityComparer() 
            : base((a, b) => a.Equals(b), a => a.GetHashCode())
        {
        }
    }
}
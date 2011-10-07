using System;
using System.Collections.Generic;

namespace ServerEngine.DataManagement
{
        /// <summary>
        ///   Taken from http://stackoverflow.com/questions/98033/wrap-a-delegate-in-an-iequalitycomparer
        /// </summary>
        public class KeyEqualityComparer<T> : IEqualityComparer<T>
        {
                private readonly Func<T, object> keyExtractor;

                public KeyEqualityComparer(Func<T, object> keyExtractor)
                {
                        this.keyExtractor = keyExtractor;
                }

                public bool Equals(T x, T y)
                {
                        return this.keyExtractor(x).Equals(this.keyExtractor(y));
                }

                public int GetHashCode(T obj)
                {
                        return this.keyExtractor(obj).GetHashCode();
                }
        }
}
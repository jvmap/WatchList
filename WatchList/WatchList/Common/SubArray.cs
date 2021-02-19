using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace WatchList.Common
{
    public static class SubArray
    {
        public static IReadOnlyCollection<T> Take<T>(T[] array, int numElements)
        {
            if (numElements < 0) throw new ArgumentOutOfRangeException(nameof(numElements));
            
            if (numElements < array.Length)
                return new _SubArray<T>(array, numElements);
            else
                return array;
        }
        
        class _SubArray<T> : IReadOnlyCollection<T>
        {
            private readonly IEnumerable<T> _enumerable;
            private readonly int _count;

            public _SubArray(T[] array, int numElements)
            {
                this._count = numElements;
                if (numElements == 0)
                    this._enumerable = Enumerable.Empty<T>(); // allows array to be GC-ed
                else
                    this._enumerable = array.Take(numElements);
            }

            public int Count => _count;

            public IEnumerator<T> GetEnumerator()
            {
                return _enumerable.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}
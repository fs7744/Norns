using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.Immutable
{
    public class ImmutableArray<T> : IImmutableArray<T>
    {
        public static readonly IImmutableArray<T> Empty = new ImmutableArray<T>(new T[0]);

        private readonly List<T> enumerable;

        public ImmutableArray(IEnumerable<T> enumerable)
        {
            this.enumerable = enumerable.ToList();
        }

        public bool IsEmpty => Count == 0;

        public int Count => enumerable.Count;

        public IEnumerator<T> GetEnumerator()
        {
            return enumerable.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
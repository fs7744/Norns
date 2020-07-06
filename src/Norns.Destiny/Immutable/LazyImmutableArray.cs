using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.Immutable
{
    public class LazyData<T>
    {
        private Func<T> func;

        public LazyData(Func<T> func)
        {
            this.func = func;
        }

        private T value;

        public T Value
        {
            get
            {
                if (func != null)
                {
                    value = func();
                    func = null;
                }
                return value;
            }
        }
    }

    public struct LazyImmutableArray<T> : IImmutableArray<T>
    {
        private readonly Lazy<List<T>> enumerable;

        public LazyImmutableArray(Func<IEnumerable<T>> init)
        {
            this.enumerable = new Lazy<List<T>>(() => init().ToList());
        }

        public int Count => enumerable.Value.Count;

        public bool IsEmpty => Count == 0;

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LazyImmutableArrayEnumerator<T>(enumerable);
        }
    }

    internal class LazyImmutableArrayEnumerator<T> : IEnumerator<T>
    {
        private readonly Lazy<List<T>> enumerable;
        private IEnumerator<T> enumerator;

        public LazyImmutableArrayEnumerator(Lazy<List<T>> enumerable)
        {
            this.enumerable = enumerable;
        }

        public T Current => enumerator.Current;

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            enumerator = null;
        }

        public bool MoveNext()
        {
            if (enumerator == null)
            {
                Reset();
            }

            return enumerator.MoveNext();
        }

        public void Reset()
        {
            enumerator = enumerable.Value.GetEnumerator();
        }
    }
}
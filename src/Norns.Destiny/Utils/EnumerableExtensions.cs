using Norns.Destiny.Immutable;
using System.Collections.Generic;

namespace System.Linq
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> InsertSeparator<T>(this IEnumerable<T> enumerate, T separator)
        {
            var enumerator = enumerate.GetEnumerator();
            if (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
            while (enumerator.MoveNext())
            {
                yield return separator;
                yield return enumerator.Current;
            }
        }

        public static IEnumerable<(int, T)> WithIndex<T>(this IEnumerable<T> enumerate)
        {
            var index = -1;
            foreach (var item in enumerate)
            {
                index++;
                yield return (index, item);
            }
        }

        public static IImmutableArray<T> EmptyImmutableArray<T>()
        {
            return ImmutableArray<T>.Empty;
        }

        public static IImmutableArray<T> ToImmutableArray<T>(this IEnumerable<T> enumerate)
        {
            return new ImmutableArray<T>(enumerate);
        }

        public static IImmutableArray<T> CreateLazyImmutableArray<T>(Func<IEnumerable<T>> enumerate)
        {
            return new LazyImmutableArray<T>(enumerate);
        }
    }
}
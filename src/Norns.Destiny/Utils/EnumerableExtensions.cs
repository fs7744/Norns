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
    }
}
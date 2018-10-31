using System.Collections.Generic;
using System.Linq;

namespace Norns.AOP.Extensions
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> SkipNull<T>(this IEnumerable<T> source)
        {
            return source.Where(i => i != null);
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Norns.Extensions.Reflection
{
    internal static class ReflectorCache<TMemberInfo, TReflector>
    {
        private static readonly ConcurrentDictionary<TMemberInfo, TReflector> dictionary = new ConcurrentDictionary<TMemberInfo, TReflector>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TReflector GetOrAdd(TMemberInfo key, Func<TMemberInfo, TReflector> factory)
        {
            return dictionary.GetOrAdd(key, k => factory(k));
        }
    }
}
using System;
using System.Collections.Concurrent;
using System.Runtime.CompilerServices;

namespace Norns.Destiny.RuntimeSymbol
{
    internal class SymbolCache<TOrigin, TSymbol>
    {
        internal readonly static ConcurrentDictionary<TOrigin, TSymbol> cache = new ConcurrentDictionary<TOrigin, TSymbol>();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TSymbol GetOrAdd(TOrigin key, Func<TOrigin, TSymbol> newSymbol)
        {
            return cache.GetOrAdd(key, newSymbol);
        }
    }

    internal class TypeSymbolCache : SymbolCache<Type, TypeSymbolInfo>
    {
        private static void AddSpecialType(Type type, string fullName)
        {
            cache.GetOrAdd(type, t => new TypeSymbolInfo(t, fullName, t.Name));
        }

        static TypeSymbolCache()
        {
            InitSpecialType();
        }

        internal static void InitSpecialType()
        {
            AddSpecialType(Type.GetType("System.Void"), "void");
            AddSpecialType(typeof(object), "object");
            AddSpecialType(typeof(bool), "bool");
            AddSpecialType(typeof(char), "char");
            AddSpecialType(typeof(short), "short");
            AddSpecialType(typeof(ushort), "ushort");
            AddSpecialType(typeof(int), "int");
            AddSpecialType(typeof(uint), "uint");
            AddSpecialType(typeof(long), "long");
            AddSpecialType(typeof(ulong), "ulong");
            AddSpecialType(typeof(decimal), "decimal");
            AddSpecialType(typeof(float), "float");
            AddSpecialType(typeof(double), "double");
            AddSpecialType(typeof(string), "string");
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static TypeSymbolInfo Get(Type key)
        {
            return cache.GetOrAdd(key, t => new TypeSymbolInfo(t));
        }
    }
}
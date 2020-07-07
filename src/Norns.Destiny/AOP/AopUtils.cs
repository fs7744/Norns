using Norns.Destiny.Structure;

namespace Norns.Destiny.AOP
{
    public static class AopUtils
    {
        public static bool CanAopType(this ITypeSymbolInfo type)
        {
            return type.IsInterface || (!type.IsSealed
                && !type.IsValueType
                && !type.IsStatic);
        }

        public static bool CanDoDefaultImplement(ITypeSymbolInfo type)
        {
            return (type.IsInterface || (type.IsClass && type.IsAbstract));
        }
    }
}
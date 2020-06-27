using Norns.Destiny.Abstraction.Structure;

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
    }
}
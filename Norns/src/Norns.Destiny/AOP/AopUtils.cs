using Norns.Destiny.Abstraction.Structure;

namespace Norns.Destiny.AOP
{
    public static class AopUtils
    {
        public static bool CanAopType(this ITypeSymbolInfo type)
        {
            return !type.IsSealed
                && !type.IsValueType
                && !type.IsStatic;
        }
    }
}
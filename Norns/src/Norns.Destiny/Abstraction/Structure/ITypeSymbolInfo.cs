namespace Norns.Destiny.Abstraction.Structure
{
    public interface ITypeSymbolInfo
    {
        string Namespace { get; }

        AccessibilityInfo Accessibility { get; }

        string Name { get; }

        bool IsStatic { get; }

        bool IsSealed { get; }
        bool IsValueType { get; }
        bool IsGenericType { get; }
        int Arity { get; }
        bool IsAbstract { get; }
        bool IsAnonymousType { get; }
    }
}
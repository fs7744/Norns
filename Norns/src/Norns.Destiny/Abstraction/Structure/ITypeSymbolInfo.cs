using System.Collections.Immutable;

namespace Norns.Destiny.Abstraction.Structure
{
    public interface ITypeSymbolInfo
    {
        object Origin { get; }
        string Namespace { get; }

        AccessibilityInfo Accessibility { get; }

        string Name { get; }

        bool IsStatic { get; }

        bool IsSealed { get; }
        bool IsValueType { get; }
        bool IsGenericType { get; }
        ImmutableArray<ITypeSymbolInfo> TypeArguments { get; }
        ImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; }
        bool IsAbstract { get; }
        bool IsAnonymousType { get; }
        bool IsClass { get; }
        bool IsInterface { get; }
    }
}
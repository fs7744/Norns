using Norns.Destiny.Immutable;

namespace Norns.Destiny.Structure
{
    public interface ITypeSymbolInfo : ISymbolInfo
    {
        string Namespace { get; }
        bool IsAbstract { get; }
        bool IsSealed { get; }
        bool IsValueType { get; }
        bool IsGenericType { get; }
        IImmutableArray<ITypeSymbolInfo> TypeArguments { get; }
        IImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; }
        bool IsAnonymousType { get; }
        bool IsClass { get; }
        bool IsInterface { get; }
        ITypeSymbolInfo BaseType { get; }
        bool IsStatic { get; }
        AccessibilityInfo Accessibility { get; }
        string GenericDefinitionName { get; }
        IImmutableArray<ITypeSymbolInfo> Interfaces { get; }
        IImmutableArray<ISymbolInfo> Members { get; }
        IImmutableArray<IAttributeSymbolInfo> Attributes { get; }
    }
}
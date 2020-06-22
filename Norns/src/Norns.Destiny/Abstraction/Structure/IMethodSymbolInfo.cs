using System.Collections.Immutable;

namespace Norns.Destiny.Abstraction.Structure
{
    public interface IMethodSymbolInfo : ISymbolInfo
    {
        ITypeSymbolInfo ReturnType { get; }
        bool IsExtensionMethod { get; }
        bool IsGenericMethod { get; }
        ImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; }
        ImmutableArray<IParameterSymbolInfo> Parameters { get; }
        bool IsStatic { get; }
        AccessibilityInfo Accessibility { get; }
    }
}
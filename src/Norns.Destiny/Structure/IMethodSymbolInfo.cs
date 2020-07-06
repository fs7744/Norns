using Norns.Destiny.Immutable;

namespace Norns.Destiny.Structure
{
    public interface IMethodSymbolInfo : ISymbolInfo
    {
        ITypeSymbolInfo ReturnType { get; }
        bool IsExtensionMethod { get; }
        bool IsGenericMethod { get; }
        AccessibilityInfo Accessibility { get; }
        bool IsStatic { get; }
        bool IsSealed { get; }
        bool IsAbstract { get; }
        bool IsOverride { get; }
        bool IsVirtual { get; }
        MethodKindInfo MethodKind { get; }
        bool IsAsync { get; }
        bool HasReturnValue { get; }
        IImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; }
        IImmutableArray<IParameterSymbolInfo> Parameters { get; }
        IImmutableArray<IAttributeSymbolInfo> Attributes { get; }
    }
}
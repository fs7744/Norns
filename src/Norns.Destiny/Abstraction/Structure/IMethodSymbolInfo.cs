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
        AccessibilityInfo Accessibility { get; }
        bool IsStatic { get; }
        bool IsSealed { get; }
        bool IsAbstract { get; }
        bool IsOverride { get; }
        bool IsVirtual { get; }
        MethodKindInfo MethodKind { get; }
        bool IsAsync { get; }
        bool HasReturnValue { get; }

        ImmutableArray<IAttributeSymbolInfo> GetAttributes();
    }
}
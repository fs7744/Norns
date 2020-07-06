using Norns.Destiny.Immutable;

namespace Norns.Destiny.Structure
{
    public interface IPropertySymbolInfo : ISymbolInfo
    {
        bool IsIndexer { get; }
        bool CanWrite { get; }
        bool CanRead { get; }
        ITypeSymbolInfo Type { get; }
        IImmutableArray<IParameterSymbolInfo> Parameters { get; }
        AccessibilityInfo Accessibility { get; }
        bool IsStatic { get; }
        bool IsExtern { get; }
        bool IsSealed { get; }
        bool IsAbstract { get; }
        bool IsOverride { get; }
        bool IsVirtual { get; }
        IMethodSymbolInfo GetMethod { get; }
        IMethodSymbolInfo SetMethod { get; }
        IImmutableArray<IAttributeSymbolInfo> Attributes { get; }
    }
}
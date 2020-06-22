using System.Collections.Immutable;

namespace Norns.Destiny.Abstraction.Structure
{
    public interface IPropertySymbolInfo : ISymbolInfo
    {
        bool IsIndexer { get; }
        bool CanWrite { get; }
        bool CanRead { get; }
        ITypeSymbolInfo Type { get; }
        ImmutableArray<IParameterSymbolInfo> Parameters { get; }
        AccessibilityInfo Accessibility { get; }
        bool IsStatic { get; }
        bool IsExtern { get; }
        bool IsSealed { get; }
        bool IsAbstract { get; }
        bool IsOverride { get; }
        bool IsVirtual { get; }
        bool IsNew { get; }
        IMethodSymbolInfo GetMethod { get; }
        IMethodSymbolInfo SetMethod { get; }
    }
}
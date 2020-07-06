using Norns.Destiny.Immutable;

namespace Norns.Destiny.Structure
{
    public interface IParameterSymbolInfo : ISymbolInfo
    {
        RefKindInfo RefKind { get; }
        bool IsParams { get; }
        bool IsOptional { get; }
        int Ordinal { get; }
        bool HasExplicitDefaultValue { get; }
        object ExplicitDefaultValue { get; }
        ITypeSymbolInfo Type { get; }
        IImmutableArray<IAttributeSymbolInfo> Attributes { get; }
    }
}
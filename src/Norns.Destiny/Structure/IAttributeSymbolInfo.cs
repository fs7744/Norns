using Norns.Destiny.Immutable;

namespace Norns.Destiny.Structure
{
    public interface IAttributeSymbolInfo : ISymbolInfo
    {
        ITypeSymbolInfo AttributeType { get; }
        IMethodSymbolInfo AttributeConstructor { get; }
        IImmutableArray<ITypedConstantInfo> ConstructorArguments { get; }
        IImmutableArray<INamedConstantInfo> NamedArguments { get; }
    }
}
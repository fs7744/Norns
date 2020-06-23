using System.Collections.Immutable;

namespace Norns.Destiny.Abstraction.Structure
{
    public interface IAttributeSymbolInfo : ISymbolInfo
    {
        ITypeSymbolInfo AttributeType { get; }
        IMethodSymbolInfo AttributeConstructor { get; }
        ImmutableArray<ITypedConstantInfo> ConstructorArguments { get; }
        ImmutableArray<INamedConstantInfo> NamedArguments { get; }
    }
}
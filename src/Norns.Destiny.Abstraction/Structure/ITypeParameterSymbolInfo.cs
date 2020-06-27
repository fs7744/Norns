using System.Collections.Immutable;

namespace Norns.Destiny.Abstraction.Structure
{
    public interface ITypeParameterSymbolInfo : ITypeSymbolInfo
    {
        int Ordinal { get; }
        VarianceKindInfo VarianceKind { get; }
        bool HasReferenceTypeConstraint { get; }
        bool HasValueTypeConstraint { get; }
        bool HasConstructorConstraint { get; }
        ImmutableArray<ITypeSymbolInfo> ConstraintTypes { get; }
    }
}
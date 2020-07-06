using Norns.Destiny.Immutable;

namespace Norns.Destiny.Structure
{
    public interface ITypeParameterSymbolInfo : ITypeSymbolInfo
    {
        int Ordinal { get; }
        RefKindInfo RefKind { get; }
        bool HasReferenceTypeConstraint { get; }
        bool HasValueTypeConstraint { get; }
        bool HasConstructorConstraint { get; }
        IImmutableArray<ITypeSymbolInfo> ConstraintTypes { get; }
    }
}
using Microsoft.CodeAnalysis;
using Norns.Destiny.Immutable;
using Norns.Destiny.Structure;
using System.Linq;

namespace Norns.Skuld.Structure
{
    public class TypeParameterSymbolInfo : TypeSymbolInfo, ITypeParameterSymbolInfo
    {
        public TypeParameterSymbolInfo(ITypeParameterSymbol type) : base(type)
        {
            Ordinal = type.Ordinal;
            HasReferenceTypeConstraint = type.HasReferenceTypeConstraint;
            HasValueTypeConstraint = type.HasValueTypeConstraint;
            HasConstructorConstraint = type.HasConstructorConstraint;
            ConstraintTypes = EnumerableExtensions.CreateLazyImmutableArray<ITypeSymbolInfo>(() => type.ConstraintTypes.Select(i => new TypeSymbolInfo(i)));
            RefKind = type.Variance.ConvertToStructure();
        }

        public int Ordinal { get; }
        public RefKindInfo RefKind { get; }
        public bool HasReferenceTypeConstraint { get; }
        public bool HasValueTypeConstraint { get; }
        public bool HasConstructorConstraint { get; }
        public IImmutableArray<ITypeSymbolInfo> ConstraintTypes { get; }
    }
}
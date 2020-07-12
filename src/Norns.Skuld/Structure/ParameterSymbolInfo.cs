using Microsoft.CodeAnalysis;
using Norns.Destiny.Immutable;
using Norns.Destiny.Structure;
using System.Linq;

namespace Norns.Skuld.Structure
{
    public class ParameterSymbolInfo : IParameterSymbolInfo
    {
        public ParameterSymbolInfo(IParameterSymbol p)
        {
            RealParameter = p;
            Type = new TypeSymbolInfo(p.Type);
            RefKind = p.RefKind.ConvertToStructure();
            Attributes = EnumerableExtensions.CreateLazyImmutableArray(() => RealParameter.GetAttributes()
            .Select(AotSymbolExtensions.ConvertToStructure)
            .Where(i => i != null));
        }

        public IParameterSymbol RealParameter { get; }
        public bool IsParams => RealParameter.IsParams;
        public bool IsOptional => RealParameter.IsOptional;
        public int Ordinal => RealParameter.Ordinal;
        public bool HasExplicitDefaultValue => RealParameter.HasExplicitDefaultValue;
        public object ExplicitDefaultValue => RealParameter.ExplicitDefaultValue;
        public object Origin => RealParameter;
        public string Name => RealParameter.Name;
        public ITypeSymbolInfo Type { get; }
        public RefKindInfo RefKind { get; }
        public string FullName => RealParameter.ToDisplayString();
        public IImmutableArray<IAttributeSymbolInfo> Attributes { get; }
    }
}
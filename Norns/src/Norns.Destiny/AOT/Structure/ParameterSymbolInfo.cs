using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Structure;
using System.Collections.Immutable;
using System.Linq;

namespace Norns.Destiny.AOT.Structure
{
    public class ParameterSymbolInfo : IParameterSymbolInfo
    {
        public ParameterSymbolInfo(IParameterSymbol p)
        {
            RealParameter = p;
            Type = new TypeSymbolInfo(p.Type);
            RefKind = p.RefKind.ConvertToStructure();
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

        public ImmutableArray<IAttributeSymbolInfo> GetAttributes() => RealParameter.GetAttributes()
            .Select(AotSymbolExtensions.ConvertToStructure)
            .Where(i => i != null)
            .ToImmutableArray();
    }
}
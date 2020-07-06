using Norns.Destiny.Immutable;
using Norns.Destiny.Structure;
using System;
using System.Linq;
using System.Reflection;

namespace Norns.Destiny.RuntimeSymbol
{
    public class ParameterSymbolInfo : IParameterSymbolInfo
    {
        public ParameterSymbolInfo(ParameterInfo p)
        {
            RealParameter = p;
            Type = p.ParameterType.GetSymbolInfo();
            RefKind = p.ConvertToStructure();
            Attributes = EnumerableExtensions.CreateLazyImmutableArray<IAttributeSymbolInfo>(() => RealParameter.GetCustomAttributesData().Select(i => new AttributeSymbolInfo(i)));
        }

        public ParameterInfo RealParameter { get; }
        public bool IsParams => Attributes.Any(i => i.AttributeType.IsType<ParamArrayAttribute>());
        public bool IsOptional => RealParameter.IsOptional;
        public int Ordinal => RealParameter.Position;
        public bool HasExplicitDefaultValue => RealParameter.HasDefaultValue;
        public object ExplicitDefaultValue => RealParameter.DefaultValue;
        public object Origin => RealParameter;
        public string Name => RealParameter.Name;
        public RefKindInfo RefKind { get; }
        public ITypeSymbolInfo Type { get; }
        public string FullName => $"{Type.FullName} {RealParameter.Name}";
        public IImmutableArray<IAttributeSymbolInfo> Attributes { get; }
    }
}
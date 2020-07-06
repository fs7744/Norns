using Norns.Destiny.Structure;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Norns.Destiny.RuntimeSymbol
{
    public class AttributeSymbolInfo : IAttributeSymbolInfo
    {
        public AttributeSymbolInfo(CustomAttributeData attributeData)
        {
            AttributeData = attributeData;
            AttributeType = attributeData.AttributeType.GetSymbolInfo();
            AttributeConstructor = new MethodSymbolInfo(attributeData.Constructor);

            ConstructorArguments = AttributeData.ConstructorArguments.Select(RuntimeSymbolExtensions.ConvertToStructure).ToImmutableArray();
            NamedArguments = AttributeData.NamedArguments.Select(RuntimeSymbolExtensions.ConvertToStructure).ToImmutableArray();
        }

        private CustomAttributeData AttributeData { get; }
        public ITypeSymbolInfo AttributeType { get; }
        public IMethodSymbolInfo AttributeConstructor { get; }
        public ImmutableArray<ITypedConstantInfo> ConstructorArguments { get; }
        public ImmutableArray<INamedConstantInfo> NamedArguments { get; }
        public object Origin => AttributeData;
        public string Name => AttributeData.ToString();
        public string FullName => Name;
    }
}
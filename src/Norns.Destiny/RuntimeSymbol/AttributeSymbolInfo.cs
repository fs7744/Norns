using Norns.Destiny.Immutable;
using Norns.Destiny.Structure;
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
            ConstructorArguments = EnumerableExtensions.CreateLazyImmutableArray(() => AttributeData.ConstructorArguments.Select(RuntimeSymbolExtensions.ConvertToStructure));
            NamedArguments = EnumerableExtensions.CreateLazyImmutableArray(() => AttributeData.NamedArguments.Select(RuntimeSymbolExtensions.ConvertToStructure));
        }

        private CustomAttributeData AttributeData { get; }
        public ITypeSymbolInfo AttributeType { get; }
        public IMethodSymbolInfo AttributeConstructor { get; }
        public object Origin => AttributeData;
        public string Name => AttributeData.ToString();
        public string FullName => Name;
        public IImmutableArray<ITypedConstantInfo> ConstructorArguments { get; }
        public IImmutableArray<INamedConstantInfo> NamedArguments { get; }
    }
}
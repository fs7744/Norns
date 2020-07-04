using Norns.Destiny.Abstraction.Structure;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Norns.Destiny.JIT.Structure
{
    public class AttributeSymbolInfo : IAttributeSymbolInfo
    {
        public AttributeSymbolInfo(CustomAttributeData attributeData)
        {
            AttributeData = attributeData;
            AttributeType = attributeData.AttributeType.GetSymbolInfo();
            AttributeConstructor = new MethodSymbolInfo(attributeData.Constructor);

            ConstructorArguments = AttributeData.ConstructorArguments.Select(JitSymbolExtensions.ConvertToStructure).ToImmutableArray();
            NamedArguments = AttributeData.NamedArguments.Select(JitSymbolExtensions.ConvertToStructure).ToImmutableArray();
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
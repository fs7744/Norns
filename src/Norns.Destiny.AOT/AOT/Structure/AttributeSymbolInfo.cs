using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Structure;
using System.Collections.Immutable;
using System.Linq;

namespace Norns.Destiny.AOT.Structure
{
    public class AttributeSymbolInfo : IAttributeSymbolInfo
    {
        public AttributeSymbolInfo(AttributeData attributeData)
        {
            AttributeData = attributeData;
            AttributeType = new TypeSymbolInfo(attributeData.AttributeClass);
            AttributeConstructor = new MethodSymbolInfo(attributeData.AttributeConstructor);
            ConstructorArguments = AttributeData.ConstructorArguments.Select(AotSymbolExtensions.ConvertToStructure).ToImmutableArray();
            NamedArguments = AttributeData.NamedArguments.Select(AotSymbolExtensions.ConvertToStructure).ToImmutableArray();
        }

        public AttributeData AttributeData { get; }
        public ITypeSymbolInfo AttributeType { get; }
        public IMethodSymbolInfo AttributeConstructor { get; }
        public ImmutableArray<ITypedConstantInfo> ConstructorArguments { get; }
        public ImmutableArray<INamedConstantInfo> NamedArguments { get; }
        public object Origin => AttributeData;
        public string Name => AttributeData.ToString();
        public string FullName => Name;
    }
}
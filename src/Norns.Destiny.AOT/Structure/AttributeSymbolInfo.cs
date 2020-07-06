using Microsoft.CodeAnalysis;
using Norns.Destiny.Immutable;
using Norns.Destiny.Structure;
using System.Linq;

namespace Norns.Skuld.Structure
{
    public class AttributeSymbolInfo : IAttributeSymbolInfo
    {
        public AttributeSymbolInfo(AttributeData attributeData)
        {
            AttributeData = attributeData;
            AttributeType = new TypeSymbolInfo(attributeData.AttributeClass);
            AttributeConstructor = attributeData.AttributeConstructor == null ? null : new MethodSymbolInfo(attributeData.AttributeConstructor);
            ConstructorArguments = EnumerableExtensions.CreateLazyImmutableArray(() => AttributeData.ConstructorArguments.Select(AotSymbolExtensions.ConvertToStructure));
            NamedArguments = EnumerableExtensions.CreateLazyImmutableArray(() => AttributeData.NamedArguments.Select(AotSymbolExtensions.ConvertToStructure));
        }

        public AttributeData AttributeData { get; }
        public ITypeSymbolInfo AttributeType { get; }
        public IMethodSymbolInfo AttributeConstructor { get; }
        public object Origin => AttributeData;
        public string Name => AttributeData.ToString();
        public string FullName => Name;
        public IImmutableArray<ITypedConstantInfo> ConstructorArguments { get; }
        public IImmutableArray<INamedConstantInfo> NamedArguments { get; }
    }
}
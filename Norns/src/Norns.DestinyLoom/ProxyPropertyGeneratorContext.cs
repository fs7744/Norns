using Microsoft.CodeAnalysis;

namespace Norns.DestinyLoom
{
    public class ProxyPropertyGeneratorContext
    {
        public ProxyPropertyGeneratorContext(IPropertySymbol property, ProxyGeneratorContext context)
        {
            Property = property;
            ClassGeneratorContext = context;
        }

        public IPropertySymbol Property { get; }
        public ProxyGeneratorContext ClassGeneratorContext { get; }

        public string GetFromDI(string type)
        {
            return ClassGeneratorContext.GetFromDI(type);
        }

        public void AddUsing(string @namespace)
        {
            ClassGeneratorContext.AddUsing(@namespace);
        }
    }
}
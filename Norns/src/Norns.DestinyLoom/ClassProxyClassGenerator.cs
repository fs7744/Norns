using Microsoft.CodeAnalysis;
using Norns.DestinyLoom.Symbols;

namespace Norns.DestinyLoom
{
    public class ClassProxyClassGenerator : AbstractProxyClassGenerator
    {
        public ClassProxyClassGenerator(IInterceptorGenerator[] interceptors) : base(interceptors)
        {
        }

        public override bool CanProxy(INamedTypeSymbol type)
        {
            return @type.TypeKind == TypeKind.Class;
        }

        public override MethodSymbol GenerateProxyMethod(ProxyMethodGeneratorContext context)
        {
            var m = base.GenerateProxyMethod(context);
            m.Symbols.AddLast(Symbol.KeyOverride);
            return m;
        }

        public override PropertySymbol GenerateProxyProperty(ProxyPropertyGeneratorContext propertyGeneratorContext)
        {
            var p = base.GenerateProxyProperty(propertyGeneratorContext);
            p.Symbols.AddLast(Symbol.KeyOverride);
            return p;
        }
    }
}
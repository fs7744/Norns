using Microsoft.CodeAnalysis;

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

        public override void GenerateProxyClass(ProxyGeneratorContext context)
        {
        }
    }
}
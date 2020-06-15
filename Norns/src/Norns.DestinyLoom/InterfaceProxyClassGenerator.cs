using Microsoft.CodeAnalysis;

namespace Norns.DestinyLoom
{
    public class InterfaceProxyClassGenerator : AbstractProxyClassGenerator
    {
        public InterfaceProxyClassGenerator(IInterceptorGenerator[] interceptors) : base(interceptors)
        {
        }

        public override bool CanProxy(INamedTypeSymbol type)
        {
            return @type.TypeKind == TypeKind.Interface;
        }
    }
}
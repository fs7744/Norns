using Microsoft.CodeAnalysis;
using Norns.DestinyLoom.Symbols;
using System.Linq;
using System.Text;

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
using Microsoft.CodeAnalysis;
using Norns.DestinyLoom.Samples;
using System.Collections.Generic;

namespace Norns.DestinyLoom.Test
{
    [Generator]
    public class ProxyGenerator : AbstractProxyGenerator
    {
        public override IEnumerable<IInterceptorGenerator> FindInterceptorGenerators()
        {
            yield return new ConsoleCallMethodGenerator();
        }

        public override bool CanProxy(INamedTypeSymbol @type)
        {
            return @type.ToDisplayString().StartsWith("Norns");
        }

        public override IEnumerable<AbstractProxyClassGenerator> FindProxyClassGenerators(IInterceptorGenerator[] interceptors)
        {
            yield return new DefaultInterfaceImplementClassGenerator(interceptors);
            yield return new InterfaceProxyClassGenerator(interceptors);
        }
    }
}
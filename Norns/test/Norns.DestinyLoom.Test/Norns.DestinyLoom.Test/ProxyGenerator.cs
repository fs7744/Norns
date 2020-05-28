using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

namespace Norns.DestinyLoom.Test
{
    [Generator]
    public class ProxyGenerator : AbstractProxyGenerator
    {
        public override IEnumerable<IInterceptorGenerator> FindInterceptorGenerators()
        {
            yield return new AddOne();
        }

        public override bool CanProxy(INamedTypeSymbol @type)
        {
            return @type.ToDisplayString().StartsWith("Norns");
        }
    }

    public class AddOne : IInterceptorGenerator
    { 
    
    }
}
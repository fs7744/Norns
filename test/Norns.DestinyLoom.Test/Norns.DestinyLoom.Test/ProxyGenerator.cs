using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOP;
using System.Collections.Generic;

namespace Norns.DestinyLoom.Test
{
    [Generator]
    public class ProxyGenerator : AotAopSourceGenerator
    {
        protected override bool Filter(ITypeSymbolInfo type)
        {
            return false;
            return base.Filter(type) && type.FullName.StartsWith("Norns");
        }

        protected override IEnumerable<IInterceptorGenerator> GetInterceptorGenerators()
        {
            yield return new ConsoleCallMethodGenerator();
        }
    }
}
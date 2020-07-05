using Microsoft.CodeAnalysis;
using Norns.Destiny.AOP;
using Norns.Destiny.AOT.AOP;
using System.Collections.Generic;

namespace Norns.DestinyLoom.Test
{
    [Generator]
    public class ProxyGenerator : AotAopSourceGenerator
    {
        protected override IEnumerable<IInterceptorGenerator> GetInterceptorGenerators()
        {
            yield return new ConsoleCallMethodGenerator();
        }
    }
}
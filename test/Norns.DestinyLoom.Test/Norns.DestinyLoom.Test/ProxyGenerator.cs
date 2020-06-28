﻿using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOP;
using Norns.Destiny.AOT.AOP;
using System.Collections.Generic;

namespace Norns.DestinyLoom.Test
{
    [Generator]
    public class ProxyGenerator : AotAopSourceGenerator
    {
        protected override bool Filter(ITypeSymbolInfo type)
        {
            return true;
            return type.FullName.StartsWith("Norns");
        }

        protected override bool FilterForDefaultImplement(ITypeSymbolInfo type)
        {
            return type.IsInterface;
        }

        protected override IEnumerable<IInterceptorGenerator> GetInterceptorGenerators()
        {
            yield return new ConsoleCallMethodGenerator();
        }
    }
}
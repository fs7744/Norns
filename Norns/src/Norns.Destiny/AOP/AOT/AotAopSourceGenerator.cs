﻿using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.AOT.Coder;
using System.Collections.Generic;

namespace Norns.Destiny.AOP
{
    public abstract class AotAopSourceGenerator : AotSourceGeneratorBase
    {
        protected abstract IEnumerable<IInterceptorGenerator> GetInterceptorGenerators();

        protected override IEnumerable<INotationGenerator> CreateNotationGenerators()
        {
            yield return new DefaultImplementNotationGenerator();
            yield return new ProxyNotationGenerator(GetInterceptorGenerators());
        }

        protected override bool Filter(ITypeSymbolInfo type)
        {
            return AopUtils.CanAopType(type);
        }
    }
}
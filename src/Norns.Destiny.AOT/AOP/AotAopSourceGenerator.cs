using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOP;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.AOT.Coder;
using System.Collections.Generic;

namespace Norns.Destiny.AOT.AOP
{
    public abstract class AotAopSourceGenerator : AotSourceGeneratorBase
    {
        protected abstract IEnumerable<IInterceptorGenerator> GetInterceptorGenerators();

        protected virtual bool FilterForDefaultImplement(ITypeSymbolInfo type)
        {
            return AopUtils.CanDoDefaultImplement(type);
        }

        protected override IEnumerable<INotationGenerator> CreateNotationGenerators()
        {
            yield return new DefaultImplementNotationGenerator(FilterForDefaultImplement);
            yield return new ProxyNotationGenerator(GetInterceptorGenerators());
        }

        protected override bool Filter(ITypeSymbolInfo type)
        {
            return AopUtils.CanAopType(type);
        }
    }
}
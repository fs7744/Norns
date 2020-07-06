using Norns.Destiny.AOP;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.Attributes;
using Norns.Destiny.Loom;
using Norns.Destiny.Structure;
using Norns.Skuld.Loom;
using System.Collections.Generic;

namespace Norns.Skuld.AOP
{
    public abstract class AopSourceGenerator : SourceGeneratorBase
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
            return type.HasAttribute<CharonAttribute>() && AopUtils.CanAopType(type);
        }
    }
}
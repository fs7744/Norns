using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOT.Coder;

namespace Norns.Destiny.AOP
{
    public abstract class AopSourceGenerator : AotSourceGeneratorBase
    {
        protected virtual bool CanAop(ITypeSymbolInfo type)
        {
            return true;
        }

        protected override INotationGenerator CreateNotationGenerator()
        {
            return new ProxyNotationGenerator(CanAop);
        }
    }
}
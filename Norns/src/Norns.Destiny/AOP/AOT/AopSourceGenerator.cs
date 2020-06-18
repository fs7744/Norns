using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.AOT.Coder;

namespace Norns.Destiny.AOP
{
    public abstract class AopSourceGenerator : AotSourceGeneratorBase
    {
        protected override INotationGenerator CreateNotationGenerator()
        {
            return new ProxyNotationGenerator();
        }
    }
}
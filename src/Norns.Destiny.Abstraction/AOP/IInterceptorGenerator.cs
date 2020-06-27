using Norns.Destiny.AOP.Notations;
using Norns.Destiny.Notations;
using System.Collections.Generic;

namespace Norns.Destiny.AOP
{
    public interface IInterceptorGenerator
    {
        IEnumerable<INotation> BeforeMethod(ProxyGeneratorContext context);

        IEnumerable<INotation> AfterMethod(ProxyGeneratorContext context);
    }
}
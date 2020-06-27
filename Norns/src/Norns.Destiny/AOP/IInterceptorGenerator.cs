using Norns.Destiny.AOP.Notations;
using Norns.Destiny.Notations;

namespace Norns.Destiny.AOP
{
    public interface IInterceptorGenerator
    {
        INotation BeforeMethod(ProxyGeneratorContext context);

        INotation AfterMethod(ProxyGeneratorContext context);
    }
}
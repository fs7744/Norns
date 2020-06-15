using Norns.DestinyLoom.Symbols;
using System.Collections.Generic;

namespace Norns.DestinyLoom
{
    public interface IInterceptorGenerator
    {
        IGenerateSymbol BeforeMethodGenerateSymbol(ProxyMethodGeneratorContext context);

        IGenerateSymbol AfterMethodGenerateSymbol(ProxyMethodGeneratorContext context);
    }

    public abstract class AbstractInterceptorGenerator : IInterceptorGenerator
    {
        public virtual IGenerateSymbol AfterMethodGenerateSymbol(ProxyMethodGeneratorContext context)
        {
            return AfterMethod(context).ToSymbol();
        }

        public virtual IGenerateSymbol BeforeMethodGenerateSymbol(ProxyMethodGeneratorContext context)
        {
            return BeforeMethod(context).ToSymbol();
        }

        public abstract IEnumerable<string> AfterMethod(ProxyMethodGeneratorContext context);

        public abstract IEnumerable<string> BeforeMethod(ProxyMethodGeneratorContext context);
    }
}
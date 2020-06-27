using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.Notations;
using System.Collections.Generic;

namespace Norns.Destiny.AOP
{
    public abstract class AbstractInterceptorGenerator : IInterceptorGenerator
    {
        public IEnumerable<INotation> BeforeMethod(ProxyGeneratorContext context)
        {
            switch (context.Symbol)
            {
                case IMethodSymbolInfo method:
                    return BeforeMethod(context, method);

                case IPropertySymbolInfo property:
                    return BeforeProperty(context, property, context.GetCurrentPropertyMethod());

                default:
                    return ConstNotations.Nothings;
            }
        }

        public virtual IEnumerable<INotation> BeforeMethod(ProxyGeneratorContext context, IMethodSymbolInfo method) => ConstNotations.Nothings;

        public virtual IEnumerable<INotation> BeforeProperty(ProxyGeneratorContext context, IPropertySymbolInfo property, IMethodSymbolInfo method) => ConstNotations.Nothings;

        public IEnumerable<INotation> AfterMethod(ProxyGeneratorContext context)
        {
            switch (context.Symbol)
            {
                case IMethodSymbolInfo method:
                    return AfterMethod(context, method);

                case IPropertySymbolInfo property:
                    return AfterProperty(context, property, context.GetCurrentPropertyMethod());

                default:
                    return ConstNotations.Nothings;
            }
        }

        public virtual IEnumerable<INotation> AfterMethod(ProxyGeneratorContext context, IMethodSymbolInfo method) => ConstNotations.Nothings;

        public virtual IEnumerable<INotation> AfterProperty(ProxyGeneratorContext context, IPropertySymbolInfo property, IMethodSymbolInfo method) => ConstNotations.Nothings;
    }
}
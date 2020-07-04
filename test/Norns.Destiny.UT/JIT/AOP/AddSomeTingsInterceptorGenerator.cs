using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOP;
using Norns.Destiny.AOP.Notations;
using Norns.Destiny.Notations;
using System.Collections.Generic;

namespace Norns.Destiny.UT.JIT.AOP
{
    public class AddSomeTingsInterceptorGenerator : AbstractInterceptorGenerator
    {
        public override IEnumerable<INotation> AfterMethod(ProxyGeneratorContext context, IMethodSymbolInfo method)
        {
            if (method.HasReturnValue)
            {
                var r = context.GetReturnValueParameterName();
                var rType = method.ReturnType;
                if (rType.IsType<long>())
                {
                    yield return r.ToNotation();
                    yield return "++;".ToNotation();
                }
                else if (rType.IsType<int>())
                {
                    yield return r.ToNotation();
                    yield return "+=5;".ToNotation();
                }
            }
        }

        public override IEnumerable<INotation> AfterProperty(ProxyGeneratorContext context, IPropertySymbolInfo property, IMethodSymbolInfo method)
        {
            if (method.HasReturnValue)
            {
                var r = context.GetReturnValueParameterName();
                var rType = method.ReturnType;
                if (rType.IsType<int>())
                {
                    yield return r.ToNotation();
                    yield return "-=5;".ToNotation();
                }
            }
        }
    }
}
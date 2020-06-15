using System;
using System.Threading.Tasks;

namespace Norns.Fate.Abstraction
{
    public abstract class AbstractInterceptorAttribute : Attribute, IInterceptor
    {
        public abstract void Invoke(FateContext context, Intercept next);

        public abstract Task InvokeAsync(FateContext context, InterceptAsync next);
    }
}
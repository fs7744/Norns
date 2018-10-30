using Norns.AOP.Abstraction.Interceptors;
using System;
using System.Threading.Tasks;

namespace Norns.AOP.Abstraction.Attributes
{
    public abstract class InterceptorBaseAttribute : Attribute, IInterceptor
    {
        public int Order => 0;

        public virtual void Intercept(InterceptContext context, Action<InterceptContext> next)
        {
            InterceptAsync(context, c =>
            {
                next(c);
                return Task.CompletedTask;
            }).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        public abstract Task InterceptAsync(InterceptContext context, Func<InterceptContext, Task> nextAsync);
    }
}
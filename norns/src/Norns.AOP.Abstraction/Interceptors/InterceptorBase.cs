using System;
using System.Threading.Tasks;

namespace Norns.AOP.Abstraction.Interceptors
{
    public abstract class InterceptorBase : IInterceptor
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
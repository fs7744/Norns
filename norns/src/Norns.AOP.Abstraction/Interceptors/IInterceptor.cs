using System;
using System.Threading.Tasks;

namespace Norns.AOP.Abstraction.Interceptors
{
    public interface IInterceptor
    {
        int Order { get; }

        void Intercept(InterceptContext context, Action<InterceptContext> next);

        Task InterceptAsync(InterceptContext context, Func<InterceptContext, Task> nextAsync);
    }
}
using Norns.AOP.Interceptors;
using Norns.AOP.Utils;
using System;
using System.Threading.Tasks;

namespace Norns.AOP.Attributes
{
    [NoIntercept]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Method | AttributeTargets.Parameter | AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public abstract class InterceptorBaseAttribute : Attribute, IInterceptor
    {
        public int Order => 0;

        public virtual void Intercept(InterceptContext context, InterceptDelegate next)
        {
            NoSynchronizationContextScope.Run(InterceptAsync(context, c =>
            {
                next(c);
                return Task.CompletedTask;
            }));
        }

        public abstract Task InterceptAsync(InterceptContext context, AsyncInterceptDelegate nextAsync);
    }
}
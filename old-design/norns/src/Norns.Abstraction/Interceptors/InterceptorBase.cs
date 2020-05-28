﻿using Norns.AOP.Attributes;
using Norns.AOP.Extensions;
using System.Threading.Tasks;

namespace Norns.AOP.Interceptors
{
    [NoIntercept]
    public abstract class InterceptorBase : IInterceptor
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
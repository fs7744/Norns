using Norns.AOP.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Norns.AOP.Core.Interceptors
{
    public class InterceptorBuilder : IInterceptorBuilder
    {
        private readonly IEnumerable<IInterceptor> interceptors;

        public InterceptorBuilder(IEnumerable<IInterceptor> interceptors)
        {
            this.interceptors = interceptors.Distinct().ToArray();
        }

        public AsyncInterceptorDelegate BuildAsyncInterceptor(MethodInfo methodInfo, AsyncInterceptorDelegate func)
        {
            var asyncFunc = func;
            var builders = interceptors
                .OrderByDescending(i => i.Order)
                .Select<IInterceptor, Func<AsyncInterceptorDelegate, AsyncInterceptorDelegate>>(i => next => async c => await i.InterceptAsync(c, next));
            foreach (var builder in builders)
            {
                asyncFunc = builder(asyncFunc);
            }
            return asyncFunc;
        }

        public InterceptorDelegate BuildInterceptor(MethodInfo methodInfo, InterceptorDelegate action)
        {
            var syncAction = action;
            var builders = interceptors
                .OrderByDescending(i => i.Order)
                .Select<IInterceptor, Func<InterceptorDelegate, InterceptorDelegate>>(i => next => c => i.Intercept(c, next));
            foreach (var builder in builders)
            {
                syncAction = builder(syncAction);
            }
            return syncAction;
        }
    }
}
using Norns.AOP.Attributes;
using Norns.AOP.Interceptors;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Norns.AOP.Core.Interceptors
{
    [NoIntercept]
    public class InterceptDelegateBuilder : IInterceptDelegateBuilder
    {
        private readonly IEnumerable<IInterceptBox> boxs;

        private readonly ConcurrentDictionary<MethodInfo, AsyncInterceptDelegate> asyncCache
            = new ConcurrentDictionary<MethodInfo, AsyncInterceptDelegate>();

        private readonly ConcurrentDictionary<MethodInfo, InterceptDelegate> syncCache
            = new ConcurrentDictionary<MethodInfo, InterceptDelegate>();

        public InterceptDelegateBuilder(IEnumerable<IInterceptBox> boxs)
        {
            this.boxs = boxs.Distinct().ToArray();
        }

        public AsyncInterceptDelegate BuildAsyncInterceptDelegate(MethodInfo methodInfo, AsyncInterceptDelegate func)
        {
            return asyncCache.GetOrAdd(methodInfo, (m) =>
            {
                var asyncFunc = func;
                var builders = boxs.Where(i => i.Verifier(m))
                    .Select(i => i.Interceptor)
                    .OrderByDescending(i => i.Order)
                    .Select<IInterceptor, Func<AsyncInterceptDelegate, AsyncInterceptDelegate>>(i => next => async c => await i.InterceptAsync(c, next));
                foreach (var builder in builders)
                {
                    asyncFunc = builder(asyncFunc);
                }
                return asyncFunc;
            });
        }

        public InterceptDelegate BuildInterceptDelegate(MethodInfo methodInfo, InterceptDelegate action)
        {
            return syncCache.GetOrAdd(methodInfo, (m) =>
            {
                var syncAction = action;
                var builders = boxs.Where(i => i.Verifier(m))
                    .Select(i => i.Interceptor)
                    .OrderByDescending(i => i.Order)
                    .Select<IInterceptor, Func<InterceptDelegate, InterceptDelegate>>(i => next => c => i.Intercept(c, next));
                foreach (var builder in builders)
                {
                    syncAction = builder(syncAction);
                }
                return syncAction;
            });
        }
    }
}
using Norns.AOP.Abstraction.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Norns.AOP.Core.Interceptors
{
    public class InterceptorChainCahce
    {
        public Action<InterceptContext> Intercept { get; }
        public Func<InterceptContext, Task> InterceptAsync { get; }

        public InterceptorChainCahce(IEnumerable<IInterceptor> interceptors,
            Action<InterceptContext> intercept, Func<InterceptContext, Task> interceptAsync)
        {
            var ois = interceptors.OrderByDescending(i => i.Order).ToArray();
            var first = ois.First();
            Action<InterceptContext> syncAction = c => first.Intercept(c, intercept);
            Func<InterceptContext, Task> asyncFunc = async c => await first.InterceptAsync(c, interceptAsync);
            foreach (var item in ois.Skip(1))
            {
                syncAction = (c) => item.Intercept(c, syncAction);
                asyncFunc = async (c) => await item.InterceptAsync(c, asyncFunc);
            }
            Intercept = syncAction;
            InterceptAsync = asyncFunc;
        }
    }
}
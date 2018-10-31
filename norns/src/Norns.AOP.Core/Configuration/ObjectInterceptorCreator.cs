using Norns.AOP.Attributes;
using Norns.AOP.Interceptors;
using System;

namespace Norns.AOP.Core.Configuration
{
    [NoIntercept]
    public class ObjectInterceptorCreator : InterceptorCreator
    {
        private readonly IInterceptor interceptor;

        public ObjectInterceptorCreator(IInterceptor interceptor,
            InterceptPredicate whitelists, InterceptPredicate blacklists) : base(interceptor.GetType(), whitelists, blacklists)
        {
            this.interceptor = interceptor;
        }

        public override IInterceptor Create(IServiceProvider serviceProvider)
        {
            return interceptor;
        }
    }
}
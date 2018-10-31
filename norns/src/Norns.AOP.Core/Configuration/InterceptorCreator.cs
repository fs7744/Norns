using Norns.AOP.Attributes;
using Norns.AOP.Configuration;
using Norns.AOP.Interceptors;
using System;

namespace Norns.AOP.Core.Configuration
{
    [NoIntercept]
    public abstract class InterceptorCreator : IInterceptorCreator
    {
        protected InterceptorCreator(Type interceptorType, InterceptPredicate whitelists, InterceptPredicate blacklists)
        {
            InterceptorType = interceptorType;
            Whitelists = whitelists;
            Blacklists = blacklists;
        }

        public Type InterceptorType { get; }
        public InterceptPredicate Whitelists { get; }
        public InterceptPredicate Blacklists { get; }

        public abstract IInterceptor Create(IServiceProvider serviceProvider);
    }
}
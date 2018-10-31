using Norns.AOP.Configuration;
using Norns.AOP.Interceptors;
using System;

namespace Norns.AOP.Core.Configuration
{
    public class InterceptorCreator : IInterceptorCreator
    {
        public Type InterceptorType { get; set; }

        public InterceptPredicate Whitelists { get; set; }

        public InterceptPredicate Blacklists { get; set; }

        public IInterceptor Create(IServiceProvider serviceProvider)
        {
            return (IInterceptor)Activator.CreateInstance(InterceptorType);
        }
    }
}
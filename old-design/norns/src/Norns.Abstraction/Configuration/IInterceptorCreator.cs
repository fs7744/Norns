using Norns.AOP.Interceptors;
using System;

namespace Norns.AOP.Configuration
{
    public interface IInterceptorCreator
    {
        Type InterceptorType { get; }
        InterceptPredicate Whitelists { get; }
        InterceptPredicate Blacklists { get; }

        IInterceptor Create(IServiceProvider serviceProvider);
    }
}
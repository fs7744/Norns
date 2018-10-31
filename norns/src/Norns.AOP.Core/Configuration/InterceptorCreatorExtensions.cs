using Norns.AOP.Core.Configuration;
using Norns.AOP.Interceptors;
using System.Collections.Generic;

namespace Norns.AOP.Configuration
{
    public static class InterceptorCreatorExtensions
    {
        public static IList<IInterceptorCreator> AddType<T>
            (this IList<IInterceptorCreator> interceptors, InterceptPredicate whitelists = null, InterceptPredicate blacklists = null) where T : IInterceptor
        {
            interceptors.Add(new InterceptorCreator()
            {
                InterceptorType = typeof(T),
                Whitelists = whitelists,
                Blacklists = blacklists
            });
            return interceptors;
        }
    }
}
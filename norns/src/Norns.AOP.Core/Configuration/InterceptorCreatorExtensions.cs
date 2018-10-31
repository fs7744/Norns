using Norns.AOP.Core.Configuration;
using Norns.AOP.Interceptors;
using System;
using System.Collections.Generic;

namespace Norns.AOP.Configuration
{
    public static class InterceptorCreatorExtensions
    {
        public static IList<IInterceptorCreator> AddType<T>
            (this IList<IInterceptorCreator> interceptors, InterceptPredicate whitelists = null, InterceptPredicate blacklists = null) where T : IInterceptor
        {
            return interceptors.AddType(typeof(T), whitelists, blacklists);
        }

        public static IList<IInterceptorCreator> AddType
            (this IList<IInterceptorCreator> interceptors, Type interceptorType, InterceptPredicate whitelists = null, InterceptPredicate blacklists = null)
        {
            interceptors.Add(new TypeInterceptorCreator(interceptorType, whitelists, blacklists));
            return interceptors;
        }

        public static IList<IInterceptorCreator> Add<T>
            (this IList<IInterceptorCreator> interceptors, Func<IServiceProvider, T> func, InterceptPredicate whitelists = null, InterceptPredicate blacklists = null) where T : IInterceptor
        {
            interceptors.Add(new FuncInterceptorCreator<T>(func, whitelists, blacklists));
            return interceptors;
        }

        public static IList<IInterceptorCreator> Add
            (this IList<IInterceptorCreator> interceptors, IInterceptor interceptor, InterceptPredicate whitelists = null, InterceptPredicate blacklists = null)
        {
            interceptors.Add(new ObjectInterceptorCreator(interceptor, whitelists, blacklists));
            return interceptors;
        }
    }
}
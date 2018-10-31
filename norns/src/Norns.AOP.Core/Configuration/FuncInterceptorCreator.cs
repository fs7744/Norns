using Norns.AOP.Interceptors;
using System;

namespace Norns.AOP.Core.Configuration
{
    public class FuncInterceptorCreator<T> : InterceptorCreator where T : IInterceptor
    {
        private readonly Func<IServiceProvider, T> createFunc;

        public FuncInterceptorCreator(Func<IServiceProvider, T> createFunc,
            InterceptPredicate whitelists, InterceptPredicate blacklists) : base(typeof(T), whitelists, blacklists)
        {
            this.createFunc = createFunc;
        }

        public override IInterceptor Create(IServiceProvider serviceProvider)
        {
            return createFunc(serviceProvider);
        }
    }
}
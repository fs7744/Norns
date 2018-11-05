using Norns.AOP.Attributes;
using System;

namespace Norns.DependencyInjection
{
    [NoIntercept]
    internal class ServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IServiceProviderEngine engine;

        public ServiceScopeFactory(IServiceProvider provider)
        {
            engine = provider as IServiceProviderEngine;
        }

        public IServiceProvider CreateScopeProvider()
        {
            return new ServiceProviderEngine(engine.Root ?? engine);
        }
    }
}
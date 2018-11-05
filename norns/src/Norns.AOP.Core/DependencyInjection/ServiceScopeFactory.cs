using System;

namespace Norns.DependencyInjection
{
    public class ServiceScopeFactory : IServiceScopeFactory
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
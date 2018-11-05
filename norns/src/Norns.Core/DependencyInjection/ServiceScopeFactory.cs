using Norns.AOP.Attributes;

namespace Norns.DependencyInjection
{
    [NoIntercept]
    internal class ServiceScopeFactory : IServiceScopeFactory
    {
        private readonly IServiceProviderEngine engine;

        public ServiceScopeFactory(INamedServiceProvider provider)
        {
            engine = provider as IServiceProviderEngine;
        }

        public INamedServiceProvider CreateScopeProvider()
        {
            return new ServiceProviderEngine(engine.Root ?? engine);
        }
    }
}
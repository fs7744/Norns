using Microsoft.Extensions.DependencyInjection;

namespace Norns.Extensions.DependencyInjection.Adapter
{
    [AOP.Attributes.NoIntercept]
    internal class ServiceScopeFactory : IServiceScopeFactory
    {
        private readonly Norns.DependencyInjection.IServiceScopeFactory serviceScopeFactory;

        public ServiceScopeFactory(Norns.DependencyInjection.IServiceScopeFactory serviceScopeFactory)
        {
            this.serviceScopeFactory = serviceScopeFactory;
        }

        public IServiceScope CreateScope()
        {
            return new ServiceScope(serviceScopeFactory.CreateScopeProvider());
        }
    }
}
using Norns.AOP.Attributes;
using System;

namespace Norns.DependencyInjection
{
    [NoIntercept]
    internal class ServiceProvider : INamedServiceProvider, IDisposable
    {
        private readonly IServiceProviderEngine engine;

        public ServiceProvider(IServiceDefintions services)
        {
            engine = new ServiceProviderEngine(services, new DelegateServiceDefintionHandler(services));
        }

        public object GetService(Type serviceType)
        {
            return GetService(serviceType, null);
        }

        public void Dispose()
        {
            engine.Dispose();
        }

        public object GetService(Type serviceType, string name)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            return engine.GetService(serviceType, name);
        }
    }
}
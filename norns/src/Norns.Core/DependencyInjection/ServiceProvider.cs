using System;

namespace Norns.DependencyInjection
{
    internal class ServiceProvider : IServiceProvider, IDisposable
    {
        private readonly IServiceProviderEngine engine;

        public ServiceProvider(IServiceDefintions services)
        {
            engine = new ServiceProviderEngine(services);
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            return engine.GetService(serviceType);
        }

        public void Dispose()
        {
            engine.Dispose();
        }

        //public void OnResolve(Type serviceType, IServiceScope serviceProviderEngineScope)
        //{
        //    throw new NotImplementedException();
        //}

        //public void OnCreate(ServiceCallSite callSite)
        //{
        //    throw new NotImplementedException();
        //}
    }
}
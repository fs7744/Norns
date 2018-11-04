using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Norns.DependencyInjection
{
    internal class ServiceProviderEngine : IServiceProviderEngine
    {
        private readonly ConcurrentDictionary<DelegateServiceDefintion, object> singletonCache;

        private ServiceDefintionFactory defintions;
        //private readonly IServiceProviderEngineCallback callback;

        //internal ServiceProviderEngineScope RootScope { get; }

        //private ConcurrentDictionary<Type, Func<ServiceProviderEngineScope, object>> RealizedServices;
        //private readonly CallSiteFactory callSiteFactory;
        ///private Func<Type, Func<ServiceProviderEngineScope, object>> createServiceAccessor;

        //public IServiceScope Root => RootScope;

        public ServiceProviderEngine(IEnumerable<ServiceDefintion> services)
        {
            singletonCache = new ConcurrentDictionary<DelegateServiceDefintion, object>();
            defintions = new ServiceDefintionFactory(services);
            //serviceTable = new ServiceTable(options.DynamicInterceptor);
            //serviceTable.Fill(services);
            //RootScope = new ServiceProviderEngineScope(this);

            //callSiteFactory = new CallSiteFactory(serviceDescriptors);
            //this.callback = callback;
            //createServiceAccessor = CreateServiceAccessor;
        }

        //public IServiceScope CreateScope()
        //{
        //    return new ServiceProviderEngineScope(this);
        //}

        public void Dispose()
        {
            //Root.Dispose();
        }

        public object GetService(Type serviceType)
        {
            var defintion = defintions.TryGet(serviceType);
            if (defintion != null)
            {
                switch (defintion.Lifetime)
                {
                    case Lifetime.Singleton:
                        return singletonCache.GetOrAdd(defintion, d => d.ImplementationFactory(this));

                    case Lifetime.Scoped:
                        return null;

                    case Lifetime.Transient:
                        return null;

                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

        //internal object GetService(Type serviceType)
        //{
        //    var realizedService = RealizedServices.GetOrAdd(serviceType, createServiceAccessor);
        //    callback?.OnResolve(serviceType, serviceProviderEngineScope);
        //    return realizedService.Invoke(serviceProviderEngineScope);
        //}

        //private Func<ServiceProviderEngineScope, object> CreateServiceAccessor(Type serviceType)
        //{
        //    var callSite = CallSiteFactory.GetCallSite(serviceType, new CallSiteChain());
        //    if (callSite != null)
        //    {
        //        callback?.OnCreate(callSite);
        //        return RealizeService(callSite);
        //    }

        //    return _ => null;
        //}

        //protected Func<ServiceProviderEngineScope, object> RealizeService(ServiceCallSite callSite)
        //{
        //    return null;
        //}
    }
}
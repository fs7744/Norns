using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Norns.DependencyInjection
{
    internal class ServiceProviderEngine : IServiceProviderEngine
    {
        private readonly ConcurrentDictionary<DelegateServiceDefintion, object> singletonCache;
        private readonly ConcurrentDictionary<DelegateServiceDefintion, object> scopedCache;
        private ServiceDefintionFactory defintions;
        internal readonly IServiceProviderEngine root;

        public ServiceProviderEngine(IEnumerable<ServiceDefintion> services)
        {
            singletonCache = new ConcurrentDictionary<DelegateServiceDefintion, object>();
            scopedCache = new ConcurrentDictionary<DelegateServiceDefintion, object>();
            defintions = new ServiceDefintionFactory(services);
            root = this;
        }

        public ServiceProviderEngine(ServiceProviderEngine root)
        {
            singletonCache = root.singletonCache;
            scopedCache = new ConcurrentDictionary<DelegateServiceDefintion, object>();
            defintions = root.defintions;
            this.root = root;
        }

        public object GetService(Type serviceType)
        {
            var defintion = defintions.TryGet(serviceType);
            if (defintion != null)
            {
                switch (defintion.Lifetime)
                {
                    case Lifetime.Singleton:
                        return singletonCache.GetOrAdd(defintion, d => d.ImplementationFactory(root));

                    case Lifetime.Scoped:
                        return scopedCache.GetOrAdd(defintion, d => d.ImplementationFactory(this));

                    case Lifetime.Transient:
                        return defintion.ImplementationFactory(this);

                    default:
                        return null;
                }
            }
            else
            {
                return null;
            }
        }

        #region IDisposable Support

        private bool disposedValue = false;

        private void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    var disposables = (root == null || root == this
                        ? singletonCache.Union(scopedCache)
                        : scopedCache)
                        .Where(x => x.Value != this);
                    foreach (var scoped in disposables)
                    {
                        var disposable = scoped.Value as IDisposable;
                        disposable?.Dispose();
                    }
                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable Support
    }
}
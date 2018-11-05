using Norns.AOP.Attributes;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Norns.DependencyInjection
{
    [NoIntercept]
    internal class ServiceProviderEngine : IServiceProviderEngine
    {
        private readonly ConcurrentDictionary<DelegateServiceDefintion, object> scopedCache;

        public ConcurrentDictionary<DelegateServiceDefintion, object> SingletonCache { get; }
        public IServiceDefintionFactory Defintions { get; }
        public IServiceProviderEngine Root { get; }

        public ServiceProviderEngine(IEnumerable<ServiceDefintion> services, IDelegateServiceDefintionHandler defintionHandler)
        {
            SingletonCache = new ConcurrentDictionary<DelegateServiceDefintion, object>();
            scopedCache = new ConcurrentDictionary<DelegateServiceDefintion, object>();
            Defintions = new ServiceDefintionFactory(services, defintionHandler);
            Root = this;
        }

        public ServiceProviderEngine(IServiceProviderEngine root)
        {
            SingletonCache = root.SingletonCache;
            scopedCache = new ConcurrentDictionary<DelegateServiceDefintion, object>();
            Defintions = root.Defintions;
            Root = root;
        }

        public object GetService(Type serviceType)
        {
            if (disposedValue)
            {
                throw new ObjectDisposedException("Has been disposed.");
            }
            var defintion = Defintions.TryGet(serviceType);
            if (defintion != null)
            {
                switch (defintion.Lifetime)
                {
                    case Lifetime.Singleton:
                        return SingletonCache.GetOrAdd(defintion, d => d.ImplementationFactory(Root));

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
                    var disposables = (Root == this
                        ? SingletonCache.Union(scopedCache)
                        : scopedCache)
                        .Where(x => x.Value != this);
                    foreach (var scoped in disposables)
                    {
                        var disposable = scoped.Value as IDisposable;
                        disposable?.Dispose();
                    }
                    scopedCache.Clear();
                    if (Root == this) SingletonCache.Clear();
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
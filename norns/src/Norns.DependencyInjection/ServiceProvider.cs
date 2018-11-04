using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;

namespace Norns.DependencyInjection
{
    internal class ServiceProvider : IServiceProvider, IDisposable
    {
        internal readonly ServiceProvider root;
        private readonly ServiceTable serviceTable;

        public ServiceProvider(IServiceCollection services, ServiceProviderOptions options)
        {
            serviceTable = new ServiceTable(options.InterceptorConfiguration?.DynamicInterceptor);
            serviceTable.Fill(services);
        }

        public ServiceProvider(ServiceProvider root)
        {
            this.root = root;
        }

        public object GetService(Type serviceType)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            ServiceDescriptor descriptor = serviceTable.GetServiceDescriptor(serviceType);
            if (descriptor == null)
            {
                return null;
            }
            return null;
            switch (descriptor.Lifetime)
            {
                case ServiceLifetime.Singleton:
                     
                    break;
                case ServiceLifetime.Scoped:
                    break;
                case ServiceLifetime.Transient:
                    break;
            }
        }

        #region IDisposable Support
        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if (root == null || root == this)
                    {
                        //foreach (var singleton in _resolvedSingletonServices.Where(x => x.Value != this))
                        //{
                        //    var disposable = singleton.Value as IDisposable;
                        //    disposable?.Dispose();
                        //}
                    }
                }
                disposedValue = true;
            }
        }
        public void Dispose()
        {
            Dispose(true);
        }
        #endregion
    }
}

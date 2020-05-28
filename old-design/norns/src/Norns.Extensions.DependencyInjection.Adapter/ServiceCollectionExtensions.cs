using Microsoft.Extensions.DependencyInjection;
using Norns.DependencyInjection;
using System;

namespace Norns.Extensions.DependencyInjection.Adapter
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceProvider BuildNornsServiceProvider(this IServiceCollection services)
        {
            return services.ToServiceDefintions()
                .AddSingleton<Microsoft.Extensions.DependencyInjection.IServiceScopeFactory, ServiceScopeFactory>()
                .BuildServiceProvider();
        }

        public static IServiceDefintions ToServiceDefintions(this IServiceCollection services)
        {
            var defintions = new ServiceDefintions();
            foreach (var service in services)
            {
                defintions.Add(service.ToServiceDefintion());
            }
            return defintions;
        }

        public static ServiceDefintion ToServiceDefintion(this ServiceDescriptor descriptor)
        {
            return ServiceDefintions.Define(descriptor.ServiceType, GetImplementationType(descriptor),
                GetLifetime(descriptor.Lifetime), GetImplementationFactory(descriptor));
        }

        private static Func<INamedServiceProvider, object> GetImplementationFactory(ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationInstance != null)
            {
                var instance = descriptor.ImplementationInstance;
                return i => instance;
            }
            else if (descriptor.ImplementationFactory != null)
            {
                var factory = descriptor.ImplementationFactory;
                return i => factory;
            }
            else
            {
                return null;
            }
        }

        private static Lifetime GetLifetime(ServiceLifetime lifetime)
        {
            switch (lifetime)
            {
                case ServiceLifetime.Singleton:
                    return Lifetime.Singleton;

                case ServiceLifetime.Scoped:
                    return Lifetime.Scoped;

                default:
                    return Lifetime.Transient;
            }
        }

        private static Type GetImplementationType(ServiceDescriptor descriptor)
        {
            if (descriptor.ImplementationType != null)
            {
                return descriptor.ImplementationType;
            }
            else if (descriptor.ImplementationInstance != null)
            {
                return descriptor.ImplementationInstance.GetType();
            }
            else if (descriptor.ImplementationFactory != null)
            {
                var typeArguments = descriptor.ImplementationFactory.GetType().GenericTypeArguments;

                return typeArguments[1];
            }
            return null;
        }
    }
}
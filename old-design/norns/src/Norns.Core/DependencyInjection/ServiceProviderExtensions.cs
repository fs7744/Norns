using System;
using System.Collections.Generic;

namespace Norns.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static object GetRequiredService(this IServiceProvider provider, Type serviceType, string name = null)
        {
            var result = provider is INamedServiceProvider named 
                ? named.GetService(serviceType, name)
                : provider.GetService(serviceType);
            if (result == null)
            {
                throw new NotSupportedException($"No found implementation for {serviceType}");
            }
            return result;
        }

        public static TService GetRequiredService<TService>(this IServiceProvider provider, string name = null) where TService : class
        {
            return (TService)provider.GetRequiredService(typeof(TService), name);
        }

        public static object GetService(this IServiceProvider provider, Type serviceType, string name = null)
        {
            return provider is INamedServiceProvider named
                ? named.GetService(serviceType, name)
                : provider.GetService(serviceType);
        }

        public static TService GetService<TService>(this IServiceProvider provider, string name = null) where TService : class
        {
            return (TService)provider.GetService(typeof(TService), name);
        }

        public static IEnumerable<TService> GetRequiredServices<TService>(this IServiceProvider provider) where TService : class
        {
            return (IEnumerable<TService>)provider.GetRequiredService(typeof(IEnumerable<TService>), null);
        }

        public static IEnumerable<TService> GetServices<TService>(this IServiceProvider provider) where TService : class
        {
            return (IEnumerable<TService>)provider.GetService(typeof(IEnumerable<TService>));
        }
    }
}
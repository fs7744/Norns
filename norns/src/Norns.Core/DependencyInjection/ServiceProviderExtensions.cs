using System;
using System.Collections.Generic;

namespace Norns.DependencyInjection
{
    public static class ServiceProviderExtensions
    {
        public static object GetRequiredService(this IServiceProvider provider, Type serviceType)
        {
            var result = provider.GetService(serviceType);
            if (result == null)
            {
                throw new NotSupportedException($"No found implementation for {serviceType}");
            }
            return result;
        }

        public static TService GetRequiredService<TService>(this IServiceProvider provider) where TService : class
        {
            return (TService)provider.GetRequiredService(typeof(TService));
        }

        public static TService GetService<TService>(this IServiceProvider provider) where TService : class
        {
            return (TService)provider.GetService(typeof(TService));
        }

        public static IEnumerable<TService> GetRequiredServices<TService>(this IServiceProvider provider) where TService : class
        {
            return (IEnumerable<TService>)provider.GetRequiredService(typeof(IEnumerable<TService>));
        }

        public static IEnumerable<TService> GetServices<TService>(this IServiceProvider provider) where TService : class
        {
            return (IEnumerable<TService>)provider.GetService(typeof(IEnumerable<TService>));
        }
    }
}
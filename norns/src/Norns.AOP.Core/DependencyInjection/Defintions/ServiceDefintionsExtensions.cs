using System;

namespace Norns.DependencyInjection
{
    public static class ServiceDefintionsExtensions
    {
        public static IServiceProvider BuildServiceProvider(this IServiceDefintions services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }

            return new ServiceProvider(services);
        }

        #region Singleton

        public static IServiceDefintions AddSingleton<TService>(this IServiceDefintions services) where TService : class
        {
            return services.AddSingleton(typeof(TService));
        }

        public static IServiceDefintions AddSingleton<TService, TImplementation>(this IServiceDefintions services)
            where TService : class where TImplementation : TService
        {
            return services.AddSingleton(typeof(TService), typeof(TImplementation));
        }

        public static IServiceDefintions AddSingleton<TService>(this IServiceDefintions services, TService service) where TService : class
        {
            return services.AddSingleton(i => service);
        }

        public static IServiceDefintions AddSingleton<TService, TImplementation>(this IServiceDefintions services,
            TImplementation implementation) where TService : class where TImplementation : TService
        {
            return services.AddSingleton<TService, TImplementation>(i => implementation);
        }

        public static IServiceDefintions AddSingleton<TService, TImplementation>(this IServiceDefintions services,
            Func<IServiceProvider, TImplementation> serviceFactory) where TService : class where TImplementation : TService
        {
            return services.AddSingleton(typeof(TService), typeof(TImplementation), i => serviceFactory(i));
        }

        public static IServiceDefintions AddSingleton<TService>(this IServiceDefintions services, Func<IServiceProvider, TService> serviceFactory) where TService : class
        {
            return services.AddSingleton(typeof(TService), typeof(TService), serviceFactory);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, object instance)
        {
            return services.AddSingleton(serviceType, serviceType, i => instance);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, Func<IServiceProvider, object> serviceFactory)
        {
            return services.AddSingleton(serviceType, serviceType, serviceFactory);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType)
        {
            return services.AddSingleton(serviceType, serviceType);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, Type implementationType)
        {
            return services.AddSingleton(serviceType, implementationType, null);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, Type implementationType, Func<IServiceProvider, object> serviceFactory)
        {
            services.Add(ServiceDefintions.Define(serviceType, implementationType, Lifetime.Singleton, serviceFactory));
            return services;
        }

        #endregion Singleton
    }
}
using System;
using System.Linq;

namespace Norns.DependencyInjection
{
    public static class ServiceDefintionsExtensions
    {
        internal static void AddInternalServiceDefintions(this IServiceDefintions services)
        {
            if (!services.Contains(typeof(IServiceProvider)))
                services.AddScoped(i => i);
            if(!services.Contains(typeof(IServiceScopeFactory)))
                services.AddScoped<IServiceScopeFactory>(i => new ServiceScopeFactory(i));
        }

        public static IServiceProvider BuildServiceProvider(this IServiceDefintions services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddInternalServiceDefintions();
            return new ServiceProvider(services);
        }

        public static IServiceDefintions Add(this IServiceDefintions services, Lifetime lifetime, Type serviceType, Type implementationType, Func<IServiceProvider, object> serviceFactory = null)
        {
            services.Add(ServiceDefintions.Define(serviceType, implementationType, lifetime, serviceFactory));
            return services;
        }

        public static IServiceDefintions AddInstance(this IServiceDefintions services, Lifetime lifetime, Type serviceType, Type implementationType, object implementationInstance)
        {
            if (implementationInstance == null)
            {
                throw new ArgumentNullException(nameof(implementationInstance));
            }
            if (!serviceType.IsInstanceOfType(implementationInstance))
            {
                throw new ArgumentException($"{implementationInstance} is not instance of type {serviceType}");
            }
            services.Add(ServiceDefintions.Define(serviceType, implementationType, lifetime, i => implementationInstance));
            return services;
        }

        public static bool Contains(this IServiceDefintions services, Type serviceType)
        {
            return services.Any(x => x.ServiceType == serviceType);
        }

        public static bool Contains(this IServiceDefintions services, Type serviceType, Type implementationType)
        {
            return services.Any(x => x.ServiceType == serviceType && x.ImplementationType == implementationType);
        }

        #region Singleton

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, Type implementationType, object implementationInstance)
        {
            return services.AddInstance(Lifetime.Singleton, serviceType, implementationType, implementationInstance);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, Type implementationType, Func<IServiceProvider, object> serviceFactory = null)
        {
            return services.Add(Lifetime.Singleton, serviceType, implementationType, serviceFactory);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType)
        {
            return services.AddSingleton(serviceType, serviceType);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, object instance)
        {
            return services.AddInstance(Lifetime.Singleton, serviceType, serviceType, instance);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, Func<IServiceProvider, object> serviceFactory)
        {
            return services.AddSingleton(serviceType, serviceType, serviceFactory);
        }

        public static IServiceDefintions AddSingleton<TService, TImplementation>(this IServiceDefintions services)
            where TService : class where TImplementation : TService
        {
            return services.AddSingleton(typeof(TService), typeof(TImplementation));
        }

        public static IServiceDefintions AddSingleton<TService, TImplementation>(this IServiceDefintions services,
            Func<IServiceProvider, TImplementation> serviceFactory) where TService : class where TImplementation : TService
        {
            return services.AddSingleton(typeof(TService), typeof(TImplementation), i => serviceFactory(i));
        }

        public static IServiceDefintions AddSingleton<TService, TImplementation>(this IServiceDefintions services,
            TImplementation implementation) where TService : class where TImplementation : TService
        {
            return services.AddInstance(Lifetime.Singleton, typeof(TService), typeof(TImplementation), implementation);
        }

        public static IServiceDefintions AddSingleton<TService>(this IServiceDefintions services) where TService : class
        {
            return services.AddSingleton(typeof(TService));
        }

        public static IServiceDefintions AddSingleton<TService>(this IServiceDefintions services, Func<IServiceProvider, TService> serviceFactory) where TService : class
        {
            return services.AddSingleton(typeof(TService), typeof(TService), serviceFactory);
        }

        public static IServiceDefintions AddSingleton<TService>(this IServiceDefintions services, TService service) where TService : class
        {
            return services.AddSingleton(typeof(TService), service);
        }

        #endregion Singleton

        #region Scoped

        public static IServiceDefintions AddScoped(this IServiceDefintions services, Type serviceType, Type implementationType,
            Func<IServiceProvider, object> serviceFactory = null)
        {
            return services.Add(Lifetime.Scoped, serviceType, implementationType, serviceFactory);
        }

        public static IServiceDefintions AddScoped(this IServiceDefintions services, Type serviceType)
        {
            return services.AddScoped(serviceType, serviceType);
        }

        public static IServiceDefintions AddScoped(this IServiceDefintions services, Type serviceType, Func<IServiceProvider, object> serviceFactory)
        {
            return services.AddScoped(serviceType, serviceType, serviceFactory);
        }

        public static IServiceDefintions AddScoped<TService, TImplementation>(this IServiceDefintions services)
   where TService : class where TImplementation : TService
        {
            return services.AddScoped(typeof(TService), typeof(TImplementation));
        }

        public static IServiceDefintions AddScoped<TService, TImplementation>(this IServiceDefintions services,
            Func<IServiceProvider, TImplementation> implementationFactory) where TService : class where TImplementation : TService
        {
            return services.AddScoped(typeof(TService), typeof(TImplementation), i => implementationFactory(i));
        }

        public static IServiceDefintions AddScoped<TService>(this IServiceDefintions services)
   where TService : class
        {
            return services.AddScoped(typeof(TService), typeof(TService));
        }

        public static IServiceDefintions AddScoped<TService>(this IServiceDefintions services, Func<IServiceProvider, TService> implementationFactory)
   where TService : class
        {
            return services.AddScoped(typeof(TService), typeof(TService), implementationFactory);
        }

        #endregion Scoped

        #region Transient

        public static IServiceDefintions AddTransient(this IServiceDefintions services, Type serviceType, Type implementationType,
            Func<IServiceProvider, object> serviceFactory = null)
        {
            return services.Add(Lifetime.Transient, serviceType, implementationType, serviceFactory);
        }

        public static IServiceDefintions AddTransient(this IServiceDefintions services, Type serviceType)
        {
            return services.AddTransient(serviceType, serviceType);
        }

        public static IServiceDefintions AddTransient(this IServiceDefintions services, Type serviceType, Func<IServiceProvider, object> serviceFactory)
        {
            return services.AddTransient(serviceType, serviceType, serviceFactory);
        }

        public static IServiceDefintions AddTransient<TService, TImplementation>(this IServiceDefintions services)
   where TService : class where TImplementation : TService
        {
            return services.AddTransient(typeof(TService), typeof(TImplementation));
        }

        public static IServiceDefintions AddTransient<TService, TImplementation>(this IServiceDefintions services,
            Func<IServiceProvider, TImplementation> implementationFactory) where TService : class where TImplementation : TService
        {
            return services.AddTransient(typeof(TService), typeof(TImplementation), i => implementationFactory(i));
        }

        public static IServiceDefintions AddTransient<TService>(this IServiceDefintions services)
   where TService : class
        {
            return services.AddTransient(typeof(TService), typeof(TService));
        }

        public static IServiceDefintions AddTransient<TService>(this IServiceDefintions services, Func<IServiceProvider, TService> implementationFactory)
   where TService : class
        {
            return services.AddTransient(typeof(TService), typeof(TService), implementationFactory);
        }

        #endregion Transient
    }
}
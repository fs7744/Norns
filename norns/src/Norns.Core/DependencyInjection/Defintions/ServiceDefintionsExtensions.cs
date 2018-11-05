using System;
using System.Linq;

namespace Norns.DependencyInjection
{
    public static class ServiceDefintionsExtensions
    {
        internal static void AddInternalServiceDefintions(this IServiceDefintions services)
        {
            if (!services.Contains<IServiceProvider>())
            {
                services.AddScoped(i => i);
            }

            if (!services.Contains<IServiceScopeFactory>())
            {
                services.AddScoped<IServiceScopeFactory>(i => new ServiceScopeFactory(i));
            }

            if (!services.Contains<IDelegateServiceDefintionHandler, PropertyInjector>())
            {
                services.AddScoped<IDelegateServiceDefintionHandler, PropertyInjector>(i => new PropertyInjector());
            }
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

        public static IServiceDefintions Add(this IServiceDefintions services, Lifetime lifetime, Type serviceType, Type implementationType, Func<INamedServiceProvider, object> serviceFactory = null, string name = null)
        {
            services.Add(ServiceDefintions.Define(serviceType, implementationType, lifetime, serviceFactory, name));
            return services;
        }

        public static IServiceDefintions AddInstance(this IServiceDefintions services, Lifetime lifetime, Type serviceType, Type implementationType, object implementationInstance, string name = null)
        {
            if (implementationInstance == null)
            {
                throw new ArgumentNullException(nameof(implementationInstance));
            }
            if (!serviceType.IsInstanceOfType(implementationInstance))
            {
                throw new ArgumentException($"{implementationInstance} is not instance of type {serviceType}");
            }
            services.Add(ServiceDefintions.Define(serviceType, implementationType, lifetime, i => implementationInstance, name));
            return services;
        }

        public static bool Contains(this IServiceDefintions services, Type serviceType)
        {
            return services.Any(x => x.ServiceType == serviceType);
        }

        public static bool Contains<TService>(this IServiceDefintions services) where TService : class
        {
            return services.Contains(typeof(TService));
        }

        public static bool Contains(this IServiceDefintions services, Type serviceType, Type implementationType)
        {
            return services.Any(x => x.ServiceType == serviceType && x.ImplementationType == implementationType);
        }

        public static bool Contains<TService, TImplementation>(this IServiceDefintions services)
            where TService : class where TImplementation : TService
        {
            return services.Contains(typeof(TService), typeof(TImplementation));
        }

        #region Singleton

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, Type implementationType, object implementationInstance, string name = null)
        {
            return services.AddInstance(Lifetime.Singleton, serviceType, implementationType, implementationInstance, name);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, Type implementationType, Func<INamedServiceProvider, object> serviceFactory = null, string name = null)
        {
            return services.Add(Lifetime.Singleton, serviceType, implementationType, serviceFactory, name);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, string name = null)
        {
            return services.AddSingleton(serviceType, serviceType, null, name);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, object instance, string name = null)
        {
            return services.AddInstance(Lifetime.Singleton, serviceType, serviceType, instance, name);
        }

        public static IServiceDefintions AddSingleton(this IServiceDefintions services, Type serviceType, Func<INamedServiceProvider, object> serviceFactory, string name = null)
        {
            return services.AddSingleton(serviceType, serviceType, serviceFactory, name);
        }

        public static IServiceDefintions AddSingleton<TService, TImplementation>(this IServiceDefintions services, string name = null)
            where TService : class where TImplementation : TService
        {
            return services.AddSingleton(typeof(TService), typeof(TImplementation), null, name);
        }

        public static IServiceDefintions AddSingleton<TService, TImplementation>(this IServiceDefintions services,
            Func<INamedServiceProvider, TImplementation> serviceFactory, string name = null) where TService : class where TImplementation : TService
        {
            return services.AddSingleton(typeof(TService), typeof(TImplementation), i => serviceFactory(i), name);
        }

        public static IServiceDefintions AddSingleton<TService, TImplementation>(this IServiceDefintions services,
            TImplementation implementation, string name = null) where TService : class where TImplementation : TService
        {
            return services.AddInstance(Lifetime.Singleton, typeof(TService), typeof(TImplementation), implementation, name);
        }

        public static IServiceDefintions AddSingleton<TService>(this IServiceDefintions services, string name = null) where TService : class
        {
            return services.AddSingleton(typeof(TService), name);
        }

        public static IServiceDefintions AddSingleton<TService>(this IServiceDefintions services, Func<INamedServiceProvider, TService> serviceFactory, string name = null) where TService : class
        {
            return services.AddSingleton(typeof(TService), typeof(TService), serviceFactory, name);
        }

        public static IServiceDefintions AddSingleton<TService>(this IServiceDefintions services, TService service, string name = null) where TService : class
        {
            return services.AddSingleton(typeof(TService), service, name);
        }

        #endregion Singleton

        #region Scoped

        public static IServiceDefintions AddScoped(this IServiceDefintions services, Type serviceType, Type implementationType,
            Func<INamedServiceProvider, object> serviceFactory = null, string name = null)
        {
            return services.Add(Lifetime.Scoped, serviceType, implementationType, serviceFactory, name);
        }

        public static IServiceDefintions AddScoped(this IServiceDefintions services, Type serviceType, string name = null)
        {
            return services.AddScoped(serviceType, serviceType, null, name);
        }

        public static IServiceDefintions AddScoped(this IServiceDefintions services, Type serviceType, Func<INamedServiceProvider, object> serviceFactory, string name = null)
        {
            return services.AddScoped(serviceType, serviceType, serviceFactory, name);
        }

        public static IServiceDefintions AddScoped<TService, TImplementation>(this IServiceDefintions services, string name = null)
   where TService : class where TImplementation : TService
        {
            return services.AddScoped(typeof(TService), typeof(TImplementation), null, name);
        }

        public static IServiceDefintions AddScoped<TService, TImplementation>(this IServiceDefintions services,
            Func<INamedServiceProvider, TImplementation> implementationFactory, string name = null) where TService : class where TImplementation : TService
        {
            return services.AddScoped(typeof(TService), typeof(TImplementation), i => implementationFactory(i), name);
        }

        public static IServiceDefintions AddScoped<TService>(this IServiceDefintions services, string name = null)
   where TService : class
        {
            return services.AddScoped(typeof(TService), typeof(TService), null, name);
        }

        public static IServiceDefintions AddScoped<TService>(this IServiceDefintions services, Func<INamedServiceProvider, TService> implementationFactory, string name = null)
   where TService : class
        {
            return services.AddScoped(typeof(TService), typeof(TService), implementationFactory, name);
        }

        #endregion Scoped

        #region Transient

        public static IServiceDefintions AddTransient(this IServiceDefintions services, Type serviceType, Type implementationType,
            Func<INamedServiceProvider, object> serviceFactory = null, string name = null)
        {
            return services.Add(Lifetime.Transient, serviceType, implementationType, serviceFactory, name);
        }

        public static IServiceDefintions AddTransient(this IServiceDefintions services, Type serviceType, string name = null)
        {
            return services.AddTransient(serviceType, serviceType, null, name);
        }

        public static IServiceDefintions AddTransient(this IServiceDefintions services, Type serviceType, Func<INamedServiceProvider, object> serviceFactory, string name = null)
        {
            return services.AddTransient(serviceType, serviceType, serviceFactory, name);
        }

        public static IServiceDefintions AddTransient<TService, TImplementation>(this IServiceDefintions services, string name = null)
   where TService : class where TImplementation : TService
        {
            return services.AddTransient(typeof(TService), typeof(TImplementation), null, name);
        }

        public static IServiceDefintions AddTransient<TService, TImplementation>(this IServiceDefintions services,
            Func<INamedServiceProvider, TImplementation> implementationFactory, string name = null) where TService : class where TImplementation : TService
        {
            return services.AddTransient(typeof(TService), typeof(TImplementation), i => implementationFactory(i), name);
        }

        public static IServiceDefintions AddTransient<TService>(this IServiceDefintions services, string name = null)
   where TService : class
        {
            return services.AddTransient(typeof(TService), typeof(TService), null, name);
        }

        public static IServiceDefintions AddTransient<TService>(this IServiceDefintions services, Func<INamedServiceProvider, TService> implementationFactory, string name = null)
   where TService : class
        {
            return services.AddTransient(typeof(TService), typeof(TService), implementationFactory, name);
        }

        #endregion Transient
    }
}
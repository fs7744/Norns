using Norns.Destiny.AOP;
using Norns.Destiny.Attributes;
using Norns.Destiny.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NornsDependencyInjectionExtensions
    {
        public static bool TryCreateProxyDescriptor(Dictionary<Type, Type> defaultInterfaceImplementDict, Dictionary<Type, Type> proxyDict, ServiceDescriptor origin, out ServiceDescriptor proxy)
        {
            proxy = origin;
            if (proxy.ImplementationType == typeof(DefaultImplementAttribute)
                && defaultInterfaceImplementDict.TryGetValue(proxy.ServiceType, out var implementType))
            {
                proxy = ServiceDescriptor.Describe(proxy.ServiceType, implementType, proxy.Lifetime);
            }
            if (proxyDict.ContainsKey(proxy.ServiceType))
            {
                proxy = ToImplementationServiceDescriptor(proxy, proxyDict[proxy.ServiceType]);
            }

            return proxy != origin;
        }

        public static IServiceProvider BuildAopServiceProvider(this IServiceCollection sc, params Assembly[] assemblies)
        {
            var (defaultInterfaceImplementDict, proxyDict) = DestinyExtensions.FindProxyTypes(AppDomain.CurrentDomain.GetAssemblies().Union(assemblies).Distinct().ToArray());

            foreach (var c in sc.ToArray())
            {
                if (TryCreateProxyDescriptor(defaultInterfaceImplementDict, proxyDict, c, out var proxy))
                {
                    sc.Remove(c);
                    sc.Add(proxy);
                }
            }
            return sc.BuildServiceProvider();
        }

        public static IServiceCollection AddDestinyInterface<T>(this IServiceCollection sc, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            sc.Add(ServiceDescriptor.Describe(typeof(T), typeof(DefaultImplementAttribute), lifetime));
            return sc;
        }

        public static ServiceDescriptor ToImplementationServiceDescriptor(ServiceDescriptor serviceDescriptor, Type implementationType)
        {
            switch (serviceDescriptor)
            {
                case ServiceDescriptor d when d.ImplementationType != null:
                    return ServiceDescriptor.Describe(serviceDescriptor.ServiceType, i =>
                    {
                        var p = ActivatorUtilities.CreateInstance(i, implementationType) as IInterceptProxy;
                        p?.SetProxy(ActivatorUtilities.CreateInstance(i, d.ImplementationType), i);
                        return p;
                    }, d.Lifetime);
                case ServiceDescriptor d when d.ImplementationFactory != null:
                    return ServiceDescriptor.Describe(serviceDescriptor.ServiceType, i =>
                    {
                        var p = ActivatorUtilities.CreateInstance(i, implementationType) as IInterceptProxy;
                        p?.SetProxy(d.ImplementationFactory(i), i);
                        return p;
                    }, d.Lifetime);
                default:
                    return ServiceDescriptor.Describe(serviceDescriptor.ServiceType, i =>
                    {
                        var p = ActivatorUtilities.CreateInstance(i, implementationType) as IInterceptProxy;
                        p?.SetProxy(serviceDescriptor.ImplementationInstance, i);
                        return p;
                    }, serviceDescriptor.Lifetime);
            }
        }
    }
}
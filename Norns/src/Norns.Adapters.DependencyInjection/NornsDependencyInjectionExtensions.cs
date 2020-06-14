using Norns.Fate;
using Norns.Fate.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NornsDependencyInjectionExtensions
    {
        public static IServiceProvider BuildAopServiceProvider(this IServiceCollection sc, params Assembly[] assemblies)
        {
            var (defaultInterfaceImplementDict, proxyDict) = FateExtensions.FindProxyTypes(assemblies);

            foreach (var c in sc.Where(c => c.ImplementationType == typeof(DefaultInterfaceImplementAttribute)).ToArray())
            {
                sc.Remove(c);
                if (defaultInterfaceImplementDict.TryGetValue(c.ServiceType, out var implementType))
                {
                    sc.Add(ServiceDescriptor.Describe(c.ServiceType, implementType, c.Lifetime));
                }
            }

            foreach (var c in sc.Where(c => proxyDict.ContainsKey(c.ServiceType)).ToArray())
            {
                sc.Remove(c);
                sc.Add(ToImplementationServiceDescriptor(c, proxyDict[c.ServiceType]));
            }

            return sc.BuildServiceProvider();
        }

        public static IServiceCollection AddFateInterface<T>(this IServiceCollection sc, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            sc.Add(ServiceDescriptor.Describe(typeof(T), typeof(DefaultInterfaceImplementAttribute), lifetime));
            return sc;
        }

        // todo: 类为代理类（ovrrve visual member） 不实现任何ioc 容器
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
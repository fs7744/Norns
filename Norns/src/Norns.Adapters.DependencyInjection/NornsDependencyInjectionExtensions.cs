using Norns.Fate.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NornsDependencyInjectionExtensions
    {
        public static (Dictionary<Type, Type> DefaultInterfaceImplementDict, Dictionary<Type, Type> ProxyDict)
            FindProxyTypes(params Assembly[] assemblies)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().Union(assemblies).Distinct().SelectMany(i => i.GetTypes())
                .Where(j => j.IsDefined(typeof(FateAttribute), false))
                .SelectMany(i => i.GetCustomAttributes<FateAttribute>()
                .Select(j =>
                {
                    switch (j)
                    {
                        case DefaultInterfaceImplementAttribute defaultInterfaceImplement:
                            return (IsProxy: false, ServiceType: defaultInterfaceImplement.InterfaceType, ImplementType: i);

                        case ProxyAttribute proxy:
                            return (IsProxy: true, ServiceType: proxy.ServiceType, ImplementType: i);

                        default:
                            return (IsProxy: false, ServiceType: null, ImplementType: null);
                    }
                }))
                .Where(i => i.ServiceType != null)
                .Distinct()
                .GroupBy(i => i.IsProxy)
                .ToDictionary(j => j.Key, i => i.GroupBy(j => j.ServiceType).ToDictionary(j => j.Key, j => j.First().ImplementType));
            Dictionary<Type, Type> defaultInterfaceImplementDict = new Dictionary<Type, Type>();
            Dictionary<Type, Type> proxyDict = new Dictionary<Type, Type>();
            foreach (var t in types)
            {
                if (t.Key)
                {
                    proxyDict = t.Value;
                }
                else
                {
                    defaultInterfaceImplementDict = t.Value;
                }
            }

            return (defaultInterfaceImplementDict, proxyDict);
        }

        public static IServiceProvider BuildAopServiceProvider(this IServiceCollection sc, params Assembly[] assemblies)
        {
            var (defaultInterfaceImplementDict, proxyDict) = FindProxyTypes(assemblies);

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
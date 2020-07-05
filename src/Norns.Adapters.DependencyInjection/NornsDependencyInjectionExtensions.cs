using Norns.Destiny.AOP;
using Norns.Destiny.Attributes;
using Norns.Destiny.JIT.AOP;
using Norns.Destiny.JIT.Coder;
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
            var serviceType = proxy.ServiceType.IsGenericType ? proxy.ServiceType.GetGenericTypeDefinition() : proxy.ServiceType;
            if (proxy.ImplementationType == typeof(DefaultImplementAttribute)
                && defaultInterfaceImplementDict.TryGetValue(serviceType, out var implementType))
            {
                proxy = ServiceDescriptor.Describe(proxy.ServiceType, proxy.ServiceType.IsGenericType ? implementType.MakeGenericType(proxy.ServiceType.GetGenericArguments()) : implementType, proxy.Lifetime);
            }
            if (proxyDict.ContainsKey(serviceType))
            {
                proxy = ToImplementationServiceDescriptor(proxy, proxy.ServiceType.IsGenericType ? proxyDict[serviceType].MakeGenericType(proxy.ServiceType.GetGenericArguments()) : proxyDict[serviceType]);
            }

            return proxy != origin;
        }

        public static IServiceProvider BuildAopServiceProvider(this IServiceCollection sc, params Assembly[] assemblies)
        {
            var (defaultInterfaceImplementDict, proxyDict) = DestinyExtensions.FindProxyTypes(assemblies.Distinct().ToArray());

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

        public static IServiceProvider BuildJitAopServiceProvider(this IServiceCollection sc, JitOptions options, IInterceptorGenerator[] interceptors, params Assembly[] assemblies)
        {
            var op = options ?? JitOptions.CreateDefault();
            var generator = new JitAopSourceGenerator(op, interceptors ?? new IInterceptorGenerator[0]);
            var assembly = generator.Generate(new JitAssembliesSymbolSource(assemblies, op.FilterProxy));
            return sc.BuildAopServiceProvider(assemblies.Union(new Assembly[] { assembly }).ToArray());
        }

        public static IServiceCollection AddDestinyInterface<T>(this IServiceCollection sc, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            return sc.AddDestinyInterface(typeof(T), lifetime);
        }

        public static IServiceCollection AddDestinyInterface(this IServiceCollection sc, Type serviceType, ServiceLifetime lifetime = ServiceLifetime.Singleton)
        {
            sc.Add(ServiceDescriptor.Describe(serviceType, typeof(DefaultImplementAttribute), lifetime));
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
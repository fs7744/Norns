using Norns.Adapters.DependencyInjection.Attributes;
using System;
using System.Linq;
using System.Reflection;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class NornsDependencyInjectionExtensions
    {
        public static IServiceProvider BuildAopServiceProvider(this IServiceCollection sc, params Assembly[] assemblies)
        {
            var ass = AppDomain.CurrentDomain.GetAssemblies().Union(assemblies).Distinct();
            var proxys = (from m in ass.SelectMany(j => j.GetCustomAttributes(typeof(ProxyMappingAttribute), false).Select(i => i as ProxyMappingAttribute))
                          join d in sc
                          on m.ServiceType equals d.ServiceType
                          select new { m.ImplementationType, m.ProxyType, ServiceDescriptor = d })
                          .Distinct()
                          .ToArray();

            foreach (var c in proxys)
            {
                sc.Remove(c.ServiceDescriptor);
                sc.Add(ToImplementationServiceDescriptor(c.ServiceDescriptor, c.ImplementationType));
                sc.Add(ServiceDescriptor.Describe(c.ServiceDescriptor.ServiceType, c.ProxyType, c.ServiceDescriptor.Lifetime));
            }

            return sc.BuildServiceProvider();
        }


        // todo: 替换所有的接口为接口实现代理类，类为代理类（ovrrve visual member） 不实现任何ioc 容器
        public static ServiceDescriptor ToImplementationServiceDescriptor(ServiceDescriptor serviceDescriptor, Type implementationType)
        {
            switch (serviceDescriptor)
            {
                case ServiceDescriptor d when d.ImplementationType != null:
                    return ServiceDescriptor.Describe(d.ImplementationType, d.ImplementationType, d.Lifetime);
                case ServiceDescriptor d when d.ImplementationFactory != null:
                    return ServiceDescriptor.Describe(implementationType, d.ImplementationFactory, d.Lifetime);
                default:
                    return ServiceDescriptor.Describe(serviceDescriptor.ImplementationInstance.GetType(), i => serviceDescriptor.ImplementationInstance, serviceDescriptor.Lifetime);
            }

        }
    }
}
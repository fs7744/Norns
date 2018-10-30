using AsyncInterceptor;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TestIOC
{
    public static class ReplaceFunc
    {
        public static IServiceCollection AddAop(this IServiceCollection services, params Assembly[] assemblies)
        {
            var interceptorTypes = AppDomain.CurrentDomain.GetAssemblies()
             .SelectMany(i =>
             {
                 try
                 {
                     return i.GetExportedTypes();
                 }
                 catch
                 {
                     return new Type[0];
                 }
             })
             .Where(i => i.IsClass && typeof(IInterceptor).IsAssignableFrom(i))
             .ToArray();

            foreach (var item in interceptorTypes)
            {
                services.AddSingleton(item);
            }

            var a = AppDomain.CurrentDomain.GetAssemblies()
             .SelectMany(i =>
             {
                 try
                 {
                     return i.GetExportedTypes();
                 }
                 catch
                 {
                     return new Type[0];
                 }
             })
             .Where(i => i.IsClass)
             .SelectMany(i => i.GetCustomAttributes<ReplaceAttribute>()
                 .SelectMany(j =>
                     i.GetInterfaces().Select(x => (x, j.RealType, i))
                     .Union(new (Type, Type, Type)[] { (i.BaseType, j.RealType, i), (i, j.RealType, i) })
                 ))
             .Where(i => i.Item1 != null && i.Item2 != null && i.Item1 != typeof(object))
             .SelectMany(i => services.Where(j => j.ServiceType == i.Item1))
              .ToArray();

            var types = AppDomain.CurrentDomain.GetAssemblies()
             .SelectMany(i =>
             {
                 try
                 {
                     return i.GetExportedTypes();
                 }
                 catch
                 {
                     return new Type[0];
                 }
             })
             .Where(i => i.IsClass)
             .SelectMany(i => i.GetCustomAttributes<ReplaceAttribute>()
                 .SelectMany(j =>
                     i.GetInterfaces().Select(x => (x, j.RealType, i))
                     .Union(new (Type, Type, Type)[] { (i.BaseType, j.RealType, i), (i, j.RealType, i) })
                 ))
             .Where(i => i.Item1 != null && i.Item2 != null && i.Item1 != typeof(object))
             .SelectMany(i=> services.Where(j => j.ServiceType == i.Item1
                         && j.ImplementationType == i.Item2)
                         .Select(j => (j, new ServiceDescriptor(j.ServiceType, i.Item3, j.Lifetime), 
                            new ServiceDescriptor(j.ImplementationType, j.ImplementationType, j.Lifetime))))
                            .ToArray();
            foreach (var item in types)
            {
                services.Remove(item.Item1);
                services.Add(item.Item3);
                services.Add(item.Item2);
            }
            return services;
        }
    }
}

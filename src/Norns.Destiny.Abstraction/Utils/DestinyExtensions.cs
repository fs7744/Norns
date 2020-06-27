using Norns.Destiny.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Norns.Destiny.Utils
{
    public static class DestinyExtensions
    {
        public static (Dictionary<Type, Type> DefaultInterfaceImplementDict, Dictionary<Type, Type> ProxyDict)
               FindProxyTypes(params Assembly[] assemblies)
        {
            var types = assemblies.Distinct().SelectMany(i => i.GetTypes())
                .Where(j => j.IsDefined(typeof(DestinyAttribute), false))
                .SelectMany(i => i.GetCustomAttributes<DestinyAttribute>()
                .Select(j =>
                {
                    switch (j)
                    {
                        case DefaultImplementAttribute defaultImplement:
                            return (IsProxy: false, ServiceType: defaultImplement.InterfaceType, ImplementType: i);

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
    }
}
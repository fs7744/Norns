using Norns.AOP.Attributes;
using Norns.AOP.Configuration;
using Norns.AOP.Extensions;
using System;
using System.Linq;
using System.Reflection;

namespace Norns.AOP.Core.Configuration
{
    [NoIntercept]
    public class InterceptorAttributeConfigurationHandler : IInterceptorConfigurationHandler
    {
        private readonly Assembly[] assemblies;

        public InterceptorAttributeConfigurationHandler(Assembly[] assemblies = null)
        {
            this.assemblies = assemblies ?? new Assembly[0];
        }

        public void Handle(IInterceptorConfiguration configuration)
        {
            var attributes = AppDomain.CurrentDomain.GetAssemblies()
                .Union(assemblies)
                .SkipNull()
                .Distinct()
                .SelectMany(i =>
                {
                    try
                    {
                        return i.ExportedTypes;
                    }
                    catch
                    {
                        return new Type[0];
                    }
                })
                .Where(i => typeof(InterceptorBaseAttribute).IsAssignableFrom(i)
                    && i != typeof(InterceptorBaseAttribute))
                .Distinct();

            foreach (var att in attributes)
            {
                configuration.Interceptors.AddType(att);
            }
        }
    }
}
using Microsoft.Extensions.DependencyInjection;
using Norns.DependencyInjection;
using System;

namespace Norns.Extensions.DependencyInjection.Adapter
{
    [AOP.Attributes.NoIntercept]
    internal class ServiceScope : IServiceScope
    {
        private readonly INamedServiceProvider NamedServiceProvider;

        public ServiceScope(INamedServiceProvider serviceProvider)
        {
            NamedServiceProvider = serviceProvider;
        }

        public IServiceProvider ServiceProvider => NamedServiceProvider;

        public void Dispose()
        {
            NamedServiceProvider.Dispose();
        }
    }

    
}
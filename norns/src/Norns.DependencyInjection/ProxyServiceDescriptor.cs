using Microsoft.Extensions.DependencyInjection;
using System;

namespace Norns.DependencyInjection
{
    internal class ProxyServiceDescriptor : ServiceDescriptor
    {
        public ProxyServiceDescriptor(ServiceDescriptor serviceDescriptor, Type proxyType)
            : base(serviceDescriptor.ServiceType, i => null, serviceDescriptor.Lifetime)
        {
            ServiceDescriptor = serviceDescriptor;
            ProxyType = proxyType;
        }

        public ServiceDescriptor ServiceDescriptor { get; }
        public Type ProxyType { get; }
    }
}
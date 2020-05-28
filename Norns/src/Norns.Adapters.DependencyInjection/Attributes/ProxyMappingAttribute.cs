using System;

namespace Norns.Adapters.DependencyInjection.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple = true, Inherited = false)]
    public class ProxyMappingAttribute : Attribute
    {
        public ProxyMappingAttribute(Type serviceType, Type implementationType, Type proxyType)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            ProxyType = proxyType;
        }

        public Type ServiceType { get; }
        public Type ImplementationType { get; }
        public Type ProxyType { get; }
    }
}
using System;

namespace Norns.Fate.Abstraction
{
    public class ProxyAttribute : FateAttribute
    {
        public ProxyAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public Type ServiceType { get; }
    }
}
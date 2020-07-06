using System;

namespace Norns.Destiny.Attributes
{
    public class ProxyAttribute : DestinyAttribute
    {
        public ProxyAttribute(Type serviceType)
        {
            ServiceType = serviceType;
        }

        public Type ServiceType { get; }
    }
}
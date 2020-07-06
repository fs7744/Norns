using System;

namespace Norns.Destiny.Attributes
{
    public class DefaultImplementAttribute : DestinyAttribute
    {
        public DefaultImplementAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }

        public Type InterfaceType { get; }
    }
}
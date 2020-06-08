using System;

namespace Norns.Fate.Abstraction
{
    public class DefaultInterfaceImplementAttribute : FateAttribute
    {
        public DefaultInterfaceImplementAttribute(Type interfaceType)
        {
            InterfaceType = interfaceType;
        }

        public Type InterfaceType { get; }
    }
}
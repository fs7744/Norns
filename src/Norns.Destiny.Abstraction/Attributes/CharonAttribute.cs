using System;

namespace Norns.Destiny.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Interface, AllowMultiple = false, Inherited = false)]
    public class CharonAttribute : Attribute
    {
    }
}
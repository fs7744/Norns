using System;

namespace Norns.AOP.Attributes
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false, Inherited = false)]
    public class NoInterceptAttribute : Attribute
    {
    }
}
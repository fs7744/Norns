using Norns.AOP.Attributes;
using System;

namespace Norns.DependencyInjection
{
    [NoIntercept]
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class FromDIAttribute : Attribute
    {
    }
}
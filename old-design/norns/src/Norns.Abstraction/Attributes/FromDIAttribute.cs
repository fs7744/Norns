using Norns.AOP.Attributes;
using System;

namespace Norns.DependencyInjection
{
    [NoIntercept]
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Parameter, AllowMultiple = false, Inherited = true)]
    public class FromDIAttribute : Attribute
    {
        public string Named { get; set; }
    }
}
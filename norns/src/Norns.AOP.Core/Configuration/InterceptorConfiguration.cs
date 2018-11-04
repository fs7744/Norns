using Norns.AOP.Attributes;
using Norns.AOP.Configuration;
using Norns.AOP.Interceptors;
using System.Collections.Generic;

namespace Norns.AOP.Core.Configuration
{
    [NoIntercept]
    public class InterceptorConfiguration : IInterceptorConfiguration
    {
        public static readonly IInterceptorConfiguration Default = new InterceptorConfiguration();

        public IList<InterceptPredicate> GlobalWhitelists { get; } = new List<InterceptPredicate>();

        public IList<InterceptPredicate> GlobalBlacklists { get; } = new List<InterceptPredicate>().AddDefaultBlacklists();

        public IList<IInterceptorCreator> Interceptors { get; } = new List<IInterceptorCreator>();

        public bool DynamicInterceptor { get ; set; }

        public bool PropertyInjector { get; set; } = true;
    }
}
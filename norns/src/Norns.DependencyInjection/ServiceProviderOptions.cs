using System.Collections.Generic;
using Norns.AOP.Configuration;
using Norns.AOP.Interceptors;

namespace Norns.DependencyInjection
{
    public class ServiceProviderOptions : IInterceptorConfiguration
    {
        public static readonly ServiceProviderOptions Default = new ServiceProviderOptions();

        public IList<InterceptPredicate> GlobalWhitelists => throw new System.NotImplementedException();

        public IList<InterceptPredicate> GlobalBlacklists => throw new System.NotImplementedException();

        public IList<IInterceptorCreator> Interceptors => throw new System.NotImplementedException();

        public bool DynamicInterceptor { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

        public  InterceptorConfiguration { get; set; }

        public bool PropertyInjector { get; set; } = true;
    }
}
using Norns.AOP.Attributes;
using Norns.AOP.Configuration;
using Norns.AOP.Core.Interceptors;
using Norns.AOP.Extensions;
using Norns.AOP.Interceptors;
using Norns.Extensions.Reflection;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.AOP.Core.Configuration
{
    [NoIntercept]
    public class InterceptorCreatorFactory : IInterceptorCreatorFactory
    {
        private static readonly InterceptPredicate defaultBlacklists = m => m.GetReflector().IsDefined<NoInterceptAttribute>()
                || m.DeclaringType.GetReflector().IsDefined<NoInterceptAttribute>();

        private static readonly InterceptPredicate defaultWhitelists = m => m.GetReflector().IsDefined<NoInterceptAttribute>()
                || m.DeclaringType.GetReflector().IsDefined<NoInterceptAttribute>();

        private readonly IInterceptorConfiguration configuration;
        private readonly IEnumerable<IInterceptorConfigurationHandler> handlers;
        private readonly IServiceProvider serviceProvider;

        public InterceptorCreatorFactory(IInterceptorConfiguration configuration,
            IEnumerable<IInterceptorConfigurationHandler> handlers, IServiceProvider serviceProvider)
        {
            this.configuration = configuration;
            this.handlers = handlers;
            this.serviceProvider = serviceProvider;
        }

        public IInterceptDelegateBuilder Build()
        {
            foreach (var handler in handlers)
            {
                handler.Handle(configuration);
            }
            return new InterceptDelegateBuilder(CreateBoxs(configuration).SkipNull());
        }

        private IEnumerable<IInterceptBox> CreateBoxs(IInterceptorConfiguration configuration)
        {
            foreach (var group in configuration.Interceptors.Distinct().GroupBy(i => i.InterceptorType))
            {
                yield return CreateBox(group, configuration.GlobalWhitelists, configuration.GlobalBlacklists);
            }
        }

        private IInterceptBox CreateBox(IGrouping<Type, IInterceptorCreator> group,
            IEnumerable<InterceptPredicate> globalWhitelists,
            IEnumerable<InterceptPredicate> globalBlacklists)
        {
            IInterceptorCreator lastCreator = null;
            var whitelists = new List<InterceptPredicate>(globalWhitelists.SkipNull());
            var blacklists = new List<InterceptPredicate>(globalBlacklists.SkipNull());
            foreach (var creator in group.SkipNull())
            {
                lastCreator = creator;
                whitelists.Add(creator.Whitelists);
                blacklists.Add(creator.Blacklists);
            }
            whitelists.Insert(0, defaultWhitelists);
            blacklists.Insert(0, defaultBlacklists);
            var white = whitelists.SkipNull().Aggregate((i, j) => m => i(m) && j(m));
            var black = blacklists.SkipNull().Aggregate((i, j) => m => i(m) || j(m));
            return lastCreator == null
                ? null
                : new InterceptBox(lastCreator.Create(serviceProvider), m => white(m) && !black(m));
        }
    }
}
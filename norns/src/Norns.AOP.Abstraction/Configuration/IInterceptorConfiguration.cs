using Norns.AOP.Interceptors;
using System.Collections.Generic;

namespace Norns.AOP.Configuration
{
    public interface IInterceptorConfiguration
    {
        IList<InterceptPredicate> GlobalWhitelists { get; }

        IList<InterceptPredicate> GlobalBlacklists { get; }

        IList<IInterceptorCreator> Interceptors { get; }
    }
}
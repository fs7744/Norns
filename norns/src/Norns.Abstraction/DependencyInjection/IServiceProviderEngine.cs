using System;
using System.Collections.Concurrent;

namespace Norns.DependencyInjection
{
    public interface IServiceProviderEngine : INamedServiceProvider
    {
        ConcurrentDictionary<DelegateServiceDefintion, object> SingletonCache { get; }
        IServiceDefintionFactory Defintions { get; }
        IServiceProviderEngine Root { get; }
    }
}
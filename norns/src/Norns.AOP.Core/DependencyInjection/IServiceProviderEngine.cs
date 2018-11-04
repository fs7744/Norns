using System;

namespace Norns.DependencyInjection
{
    public interface IServiceProviderEngine : IDisposable, IServiceProvider //, IServiceScopeFactory
    {
        //IServiceScope Root { get; }
    }
}
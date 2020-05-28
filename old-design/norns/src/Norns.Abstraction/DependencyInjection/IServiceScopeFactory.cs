using System;

namespace Norns.DependencyInjection
{
    public interface IServiceScopeFactory
    {
        INamedServiceProvider CreateScopeProvider();
    }
}
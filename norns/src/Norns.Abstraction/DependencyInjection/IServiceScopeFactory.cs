using System;

namespace Norns.DependencyInjection
{
    public interface IServiceScopeFactory
    {
        IServiceProvider CreateScopeProvider();
    }
}
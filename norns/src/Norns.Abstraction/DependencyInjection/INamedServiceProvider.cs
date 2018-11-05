using System;

namespace Norns.DependencyInjection
{
    public interface INamedServiceProvider : IServiceProvider
    {
        object GetService(Type serviceType, string name);
    }
}
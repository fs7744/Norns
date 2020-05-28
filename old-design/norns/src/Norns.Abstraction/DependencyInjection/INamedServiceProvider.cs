using System;

namespace Norns.DependencyInjection
{
    public interface INamedServiceProvider : IServiceProvider, IDisposable
    {
        object GetService(Type serviceType, string name);
    }
}
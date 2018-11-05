using System;

namespace Norns.DependencyInjection
{
    public interface IServiceDefintionFactory
    {
        DelegateServiceDefintion TryGet(Type serviceType);
    }
}
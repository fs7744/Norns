using System;

namespace Norns.DependencyInjection
{
    public interface IImplementationFactory
    {
        Func<IServiceProvider, object> ImplementationFactory { get; }
    }
}
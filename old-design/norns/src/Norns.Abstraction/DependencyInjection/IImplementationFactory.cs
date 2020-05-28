using System;

namespace Norns.DependencyInjection
{
    public interface IImplementationFactory
    {
        Func<INamedServiceProvider, object> ImplementationFactory { get; }
    }
}
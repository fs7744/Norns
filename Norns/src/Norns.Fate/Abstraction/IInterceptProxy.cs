using System;

namespace Norns.Fate.Abstraction
{
    public interface IInterceptProxy
    {
        void SetProxy(object instance, IServiceProvider serviceProvider);
    }
}
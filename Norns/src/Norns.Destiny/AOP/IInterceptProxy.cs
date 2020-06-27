using System;

namespace Norns.Destiny.AOP
{
    public interface IInterceptProxy
    {
        void SetProxy(object instance, IServiceProvider serviceProvider);
    }
}
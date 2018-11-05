using Norns.AOP.Interceptors;

namespace Norns.AOP.Configuration
{
    public interface IInterceptorCreatorFactory
    {
        IInterceptDelegateBuilder Build();
    }
}
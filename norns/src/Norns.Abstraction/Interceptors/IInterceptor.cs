using Norns.AOP.Attributes;
using System.Threading.Tasks;

namespace Norns.AOP.Interceptors
{
    [NoIntercept]
    public interface IInterceptor
    {
        int Order { get; }

        void Intercept(InterceptContext context, InterceptDelegate next);

        Task InterceptAsync(InterceptContext context, AsyncInterceptDelegate nextAsync);
    }
}
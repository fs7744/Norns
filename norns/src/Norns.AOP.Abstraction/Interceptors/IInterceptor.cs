using System.Threading.Tasks;

namespace Norns.AOP.Interceptors
{
    public interface IInterceptor
    {
        int Order { get; }

        void Intercept(InterceptContext context, InterceptorDelegate next);

        Task InterceptAsync(InterceptContext context, AsyncInterceptorDelegate nextAsync);
    }
}
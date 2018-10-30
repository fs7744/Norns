using System.Reflection;

namespace Norns.AOP.Interceptors
{
    public interface IInterceptorBuilder
    {
        InterceptorDelegate BuildInterceptor(MethodInfo methodInfo, InterceptorDelegate action);

        AsyncInterceptorDelegate BuildAsyncInterceptor(MethodInfo methodInfo, AsyncInterceptorDelegate func);
    }
}
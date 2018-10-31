using Norns.AOP.Attributes;
using System.Reflection;

namespace Norns.AOP.Interceptors
{
    [NoIntercept]
    public interface IInterceptDelegateBuilder
    {
        InterceptDelegate BuildInterceptDelegate(MethodInfo methodInfo, InterceptDelegate action);

        AsyncInterceptDelegate BuildAsyncInterceptDelegate(MethodInfo methodInfo, AsyncInterceptDelegate func);
    }
}
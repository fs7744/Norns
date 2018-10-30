using System.Threading.Tasks;

namespace Norns.AOP.Interceptors
{
    public delegate void InterceptorDelegate(InterceptContext context);

    public delegate Task AsyncInterceptorDelegate(InterceptContext context);
}
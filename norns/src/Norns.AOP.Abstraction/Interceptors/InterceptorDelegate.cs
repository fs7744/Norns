using System.Reflection;
using System.Threading.Tasks;

namespace Norns.AOP.Interceptors
{
    public delegate void InterceptDelegate(InterceptContext context);

    public delegate Task AsyncInterceptDelegate(InterceptContext context);

    public delegate bool InterceptPredicate(MethodInfo method);
}
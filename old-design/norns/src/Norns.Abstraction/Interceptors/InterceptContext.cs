using Norns.AOP.Attributes;
using Norns.DependencyInjection;
using System.Reflection;

namespace Norns.AOP.Interceptors
{
    [NoIntercept]
    public struct InterceptContext
    {
        public Additions Additions;

        public MethodInfo ServiceMethod;

        public INamedServiceProvider ServiceProvider;

        public object[] Parameters;

        public object Result;
    }
}
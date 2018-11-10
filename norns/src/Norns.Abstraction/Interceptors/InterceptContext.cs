using Norns.AOP.Attributes;
using Norns.DependencyInjection;
using System.Collections.Generic;
using System.Reflection;

namespace Norns.AOP.Interceptors
{
    [NoIntercept]
    public struct InterceptContext
    {
        public Dictionary<string, object> Additions;

        public MethodBase ServiceMethod;

        public INamedServiceProvider ServiceProvider;

        public object[] Parameters;

        public object Result;
    }
}
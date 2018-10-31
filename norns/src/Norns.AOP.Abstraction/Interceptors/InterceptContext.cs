using Norns.AOP.Attributes;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace Norns.AOP.Interceptors
{
    [NoIntercept]
    public class InterceptContext
    {
        public Dictionary<string, object> Additions { get; set; } = new Dictionary<string, object>();

        public MethodBase ServiceMethod { get; set; }

        public IServiceProvider ServiceProvider { get; set; }

        public object[] Parameters { get; set; }

        public object Result { get; set; }
    }
}
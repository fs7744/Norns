using System.Collections.Generic;
using System.Reflection;

namespace Norns.AOP.Interceptors
{
    public class InterceptContext
    {
        public Dictionary<string, object> Additions { get; set; } = new Dictionary<string, object>();

        public MethodBase ServiceMethod { get; set; }

        public object[] Parameters { get; set; }

        public object Result { get; set; }
    }
}
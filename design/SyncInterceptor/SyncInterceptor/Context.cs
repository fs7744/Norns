using System.Collections.Generic;
using System.Reflection;

namespace SyncInterceptor
{
    public class Context
    {
        public Dictionary<string, object> Additions { get; set; } = new Dictionary<string, object>();

        public MethodBase ServiceMethod { get; set; }

        public object[] Parameters { get; set; }

        public object Result { get; set; }
    }
}
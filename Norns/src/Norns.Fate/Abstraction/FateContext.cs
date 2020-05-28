using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Norns.Fate.Abstraction
{
    public class FateContext
    {
        public IServiceProvider Provider { get; set; }
        public MethodInfo Method { get; set; }
        public object[] Parameters { get; set; }
        public object ReturnValue { get; set; }
    }
}
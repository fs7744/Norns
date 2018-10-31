using System.Reflection;

namespace Norns.Extensions.Reflection
{
    public class ParameterReflector
    {
        public ParameterInfo Member { get; }

        public ParameterReflector(ParameterInfo member)
        {
            Member = member;
        }
    }
}
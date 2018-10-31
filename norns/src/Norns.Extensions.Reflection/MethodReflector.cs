using System.Reflection;

namespace Norns.Extensions.Reflection
{
    public class MethodReflector : Reflector<MethodInfo>
    {
        public MethodReflector(MethodInfo member) : base(member, member.CustomAttributes)
        {
        }
    }
}
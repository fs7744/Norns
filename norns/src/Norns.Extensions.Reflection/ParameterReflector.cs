using System.Reflection;

namespace Norns.Extensions.Reflection
{
    public class ParameterReflector : Reflector<ParameterInfo>
    {
        public ParameterReflector(ParameterInfo member) : base(member, member.CustomAttributes)
        {
        }
    }
}
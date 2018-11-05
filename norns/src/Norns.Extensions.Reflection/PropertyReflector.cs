using System.Reflection;

namespace Norns.Extensions.Reflection
{
    public class PropertyReflector : Reflector<PropertyInfo>
    {
        public PropertyReflector(PropertyInfo member) : base(member, member.CustomAttributes)
        {
        }
    }
}
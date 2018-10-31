using System;

namespace Norns.Extensions.Reflection
{
    public class TypeReflector : Reflector<Type>
    {
        public TypeReflector(Type member) : base(member, member.CustomAttributes)
        {
        }
    }
}
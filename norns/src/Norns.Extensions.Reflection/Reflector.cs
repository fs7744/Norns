using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Norns.Extensions.Reflection
{
    public class Reflector<T> where T : MemberInfo
    {
        private readonly CustomAttributeCreator[] customAttributeCreators;

        public Reflector(T member)
        {
            customAttributeCreators = member.CustomAttributes.Select(i => new CustomAttributeCreator(i)).ToArray();
        }

        public bool IsDefined<TAttribute>() where TAttribute : Attribute
        {
            return IsDefined(typeof(TAttribute));
        }

        public bool IsDefined(Type attributeType)
        {
            var attrToken = attributeType.TypeHandle;
            return customAttributeCreators.Any(i => i.Tokens.Contains(attrToken));
        }

        public IEnumerable<TAttribute> GetCustomAttributes<TAttribute>() where TAttribute : Attribute
        {
            return GetCustomAttributes(typeof(TAttribute)).Select(i => i as TAttribute);
        }

        public IEnumerable<Attribute> GetCustomAttributes(Type attributeType)
        {
            var attrToken = attributeType.TypeHandle;
            return customAttributeCreators.Where(i => i.Tokens.Contains(attrToken))
                .Select(i => i.Create());
        }

        public TAttribute GetCustomAttribute<TAttribute>() where TAttribute : Attribute
        {
            return GetCustomAttributes<TAttribute>().FirstOrDefault();
        }

        public Attribute GetCustomAttribute(Type attributeType)
        {
            return GetCustomAttributes(attributeType).FirstOrDefault();
        }
    }
}
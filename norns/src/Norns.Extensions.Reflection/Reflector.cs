using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Norns.Extensions.Reflection
{
    public class Reflector<T> where T : ICustomAttributeProvider
    {
        public T Member { get; }

        public CustomAttributeReflector[] CustomAttributeReflectors { get; }

        public Reflector(T member, IEnumerable<CustomAttributeData> customAttributes)
        {
            CustomAttributeReflectors = customAttributes.Select(i => new CustomAttributeReflector(i)).ToArray();
            Member = member;
        }

        public bool IsDefined<TAttribute>() where TAttribute : Attribute
        {
            return IsDefined(typeof(TAttribute));
        }

        public bool IsDefined(Type attributeType)
        {
            var attrToken = attributeType.TypeHandle;
            return CustomAttributeReflectors.Any(i => i.Tokens.Contains(attrToken));
        }

        public IEnumerable<TAttribute> GetCustomAttributes<TAttribute>() where TAttribute : Attribute
        {
            return GetCustomAttributes(typeof(TAttribute)).Select(i => i as TAttribute);
        }

        public IEnumerable<Attribute> GetCustomAttributes(Type attributeType)
        {
            var attrToken = attributeType.TypeHandle;
            return CustomAttributeReflectors.Where(i => i.Tokens.Contains(attrToken))
                .Select(i => i.Invoke());
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
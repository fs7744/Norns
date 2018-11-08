using Mono.Cecil;
using System;

namespace Norns.StaticWeave
{
    public static class TypeReferenceExtensions
    {
        public static bool IsType(this TypeReference reference, Type type)
        {
            return reference.FullName == type.FullName;
        }

        public static bool BaseTypeIs(this TypeReference reference, Type type)
        {
            var result = false;
            var typeDefinition = reference as TypeDefinition;
            while (typeDefinition != null
                && typeDefinition.BaseType != null
                && !result)
            {
                result = typeDefinition.BaseType.IsType(type);
            }
            return result;
        }
    }
}
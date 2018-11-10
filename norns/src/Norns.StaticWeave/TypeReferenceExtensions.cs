using Mono.Cecil;
using Mono.Cecil.Cil;
using Norns.AOP.Attributes;
using Norns.AOP.Interceptors;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.StaticWeave
{
    public static class TypeReferenceExtensions
    {
        public const string CtorName = ".ctor";
        public const string StaticCtorName = ".cctor";
        public static readonly MethodAttributes StaticCtorAttributes = MethodAttributes.Static | MethodAttributes.HideBySig | MethodAttributes.SpecialName | MethodAttributes.RTSpecialName;
        public static readonly Type ObjectType = typeof(object);
        public static readonly Type NoInterceptAttributeType = typeof(NoInterceptAttribute);
        public static readonly Type InterceptorBaseAttributeType = typeof(InterceptorBaseAttribute);
        public static readonly Type InterceptorBaseType = typeof(InterceptorBase);
        public static readonly Type IInterceptorType = typeof(IInterceptor);
        public static readonly HashSet<string> DefaultNoInterceptMethods = new HashSet<string>()
        {
            "Equals",
            "GetHashCode",
            "ToString",
            "GetType",
            "Finalize",
            ".ctor"
        };

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
                typeDefinition = typeDefinition.BaseType as TypeDefinition;
            }
            return result;
        }

        public static bool InterfaceIs(this TypeDefinition typeDefinition, Type type)
        {
            return !typeDefinition.HasInterfaces 
                && typeDefinition.Interfaces.Any(i => i.InterfaceType.TypeOrBaseTypeIs(type));
        }

        public static bool TypeOrBaseTypeIs(this TypeReference reference, Type type)
        {
            return reference.IsType(type)
                || reference.BaseTypeIs(type);
        }

        public static bool NeedIntercept(this ICustomAttributeProvider def)
        {
            return !def.HasCustomAttributes 
                || def.CustomAttributes.All(j => !j.AttributeType.IsType(NoInterceptAttributeType));
        }

        public static IEnumerable<TypeDefinition> FindNeedInterceptTypes(this AssemblyDefinition assembly)
        {
            return assembly.Modules.SelectMany(i => i.Types)
                .Where(i => i.IsPublic && i.IsClass && !i.IsAbstract 
                    && !i.FullName.EndsWith("Attribute") 
                    && i.NeedIntercept()
                    && !i.TypeOrBaseTypeIs(InterceptorBaseType)
                    && !i.IsType(ObjectType));
        }

        public static IEnumerable<MethodDefinition> FindNeedInterceptMethods(this TypeDefinition typeDefinition)
        {
            return typeDefinition.Methods
                .Where(i => i.IsPublic && !i.IsStatic && !i.IsAbstract && i.NeedIntercept()
                && !DefaultNoInterceptMethods.Contains(i.Name));
        }

        public static MethodDefinition FindStaticCtorMethod(this TypeDefinition typeDefinition)
        {
            return typeDefinition.Methods
                .FirstOrDefault(i => i.Name ==  StaticCtorName);
        }

        public static void InsertBeforeLast(this MethodDefinition methodDefinition, Instruction instruction)
        {
            var instrs = methodDefinition.Body.Instructions;
            instrs.Insert(instrs.Count > 0 ? instrs.Count - 1 : 0, instruction);
        }
    }
}
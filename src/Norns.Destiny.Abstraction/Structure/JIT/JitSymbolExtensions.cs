using Norns.Destiny.Abstraction.Structure;
using System;
using System.Reflection;

namespace Norns.Destiny.JIT.Structure
{
    public static class JitSymbolExtensions
    {
        public static RefKindInfo ConvertToStructure(this GenericParameterAttributes attributes)
        {
            if ((attributes & GenericParameterAttributes.Covariant) == GenericParameterAttributes.Covariant)
            {
                return RefKindInfo.Out;
            }
            else if ((attributes & GenericParameterAttributes.Contravariant) == GenericParameterAttributes.Contravariant)
            {
                return RefKindInfo.In;
            }
            else
            {
                return RefKindInfo.None;
            }
        }

        public static RefKindInfo ConvertToStructure(this ParameterInfo p)
        {
            if (p.IsOut)
            {
                return RefKindInfo.Out;
            }
            else if (p.IsIn)
            {
                return RefKindInfo.In;
            }
            else if (p.IsRetval || (p.ParameterType.ContainsGenericParameters && p.ParameterType.IsByRef))
            {
                return RefKindInfo.Ref;
            }
            else
            {
                return RefKindInfo.None;
            }
        }

        public static ISymbolInfo ConvertToStructure(this MemberInfo member)
        {
            switch (member)
            {
                case FieldInfo f:
                    return new FieldSymbolInfo(f);

                case MethodBase m:
                    return new MethodSymbolInfo(m);

                case PropertyInfo p:
                    return new PropertySymbolInfo(p);

                default:
                    return null;
            }
        }

        public static ITypedConstantInfo ConvertToStructure(this CustomAttributeTypedArgument arg)
        {
            return new TypedConstantInfo()
            {
                Type = arg.ArgumentType.GetSymbolInfo(),
                Value = arg.Value
            };
        }

        public static INamedConstantInfo ConvertToStructure(this CustomAttributeNamedArgument arg)
        {
            return new TypedConstantInfo()
            {
                Type = arg.TypedValue.ArgumentType.GetSymbolInfo(),
                Value = arg.TypedValue.Value,
                Name = arg.MemberName
            };
        }

        public static AccessibilityInfo ConvertAccessibilityInfo(this Type type)
        {
            if (type.IsPublic || (type.IsNested && type.IsNestedPublic))
            {
                return AccessibilityInfo.Public;
            }
            else if (type.IsNotPublic || (type.IsNested && type.IsNestedAssembly))
            {
                return AccessibilityInfo.Internal;
            }
            else if (type.IsNested && type.IsNestedFamily)
            {
                return AccessibilityInfo.Protected;
            }
            else if (type.IsNested && type.IsNestedPrivate)
            {
                return AccessibilityInfo.Private;
            }
            else if (type.IsNested && type.IsNestedFamANDAssem)
            {
                return AccessibilityInfo.ProtectedAndInternal;
            }
            else if (type.IsNested && type.IsNestedFamORAssem)
            {
                return AccessibilityInfo.ProtectedOrInternal;
            }
            else
            {
                return AccessibilityInfo.NotApplicable;
            }
        }

        public static AccessibilityInfo ConvertAccessibilityInfo(this FieldInfo type)
        {
            if (type.IsPublic)
            {
                return AccessibilityInfo.Public;
            }
            else if (type.IsPrivate)
            {
                return AccessibilityInfo.Private;
            }
            else if (type.IsAssembly)
            {
                return AccessibilityInfo.Internal;
            }
            else if (type.IsFamily)
            {
                return AccessibilityInfo.Protected;
            }
            else if (type.IsFamilyAndAssembly)
            {
                return AccessibilityInfo.ProtectedAndInternal;
            }
            else if (type.IsFamilyOrAssembly)
            {
                return AccessibilityInfo.ProtectedOrInternal;
            }
            else
            {
                return AccessibilityInfo.NotApplicable;
            }
        }

        public static AccessibilityInfo ConvertAccessibilityInfo(this MethodBase method)
        {
            if (method.IsPublic)
            {
                return AccessibilityInfo.Public;
            }
            else if (method.IsPrivate)
            {
                return AccessibilityInfo.Private;
            }
            else if (method.IsAssembly)
            {
                return AccessibilityInfo.Internal;
            }
            else if (method.IsFamily)
            {
                return AccessibilityInfo.Protected;
            }
            else if (method.IsFamilyAndAssembly)
            {
                return AccessibilityInfo.ProtectedAndInternal;
            }
            else if (method.IsFamilyOrAssembly)
            {
                return AccessibilityInfo.ProtectedOrInternal;
            }
            else
            {
                return AccessibilityInfo.NotApplicable;
            }
        }

        public static MethodKindInfo ConvertMethodKindInfo(this MethodBase method)
        {
            if (method.IsConstructor)
            {
                return method.IsStatic ? MethodKindInfo.StaticConstructor : MethodKindInfo.Constructor;
            }
            else if (method.IsSpecialName)
            {
                if (method.Name.StartsWith("get_"))
                {
                    return MethodKindInfo.PropertyGet;
                }
                else if (method.Name.StartsWith("set_"))
                {
                    return MethodKindInfo.PropertySet;
                }
                else if (method.Name.StartsWith("add_"))
                {
                    return MethodKindInfo.EventAdd;
                }
                else if (method.Name.StartsWith("remove_"))
                {
                    return MethodKindInfo.EventRemove;
                }
                else if (method.Name.StartsWith("raise_"))
                {
                    return MethodKindInfo.EventRaise;
                }
                else
                {
                    return MethodKindInfo.AnonymousFunction;
                }
            }
            else
            {
                return MethodKindInfo.Method;
            }
        }
    }
}
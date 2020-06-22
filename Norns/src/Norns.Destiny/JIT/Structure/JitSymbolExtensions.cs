﻿using Norns.Destiny.Abstraction.Structure;
using System;
using System.Reflection;

namespace Norns.Destiny.JIT.Structure
{
    public static class JitSymbolExtensions
    {
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

        public static VarianceKindInfo ConvertToStructure(this GenericParameterAttributes attributes)
        {
            if ((attributes & GenericParameterAttributes.Covariant) == GenericParameterAttributes.Covariant)
            {
                return VarianceKindInfo.Out;
            }
            else if ((attributes & GenericParameterAttributes.Contravariant) == GenericParameterAttributes.Contravariant)
            {
                return VarianceKindInfo.In;
            }
            else
            {
                return VarianceKindInfo.None;
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
            else if (p.IsRetval)
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
                case MethodInfo m:
                    return new MethodSymbolInfo(m);
                case PropertyInfo p:
                    return new PropertySymbolInfo(p);
                default:
                    return null;
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
    }
}
using Norns.Destiny.Abstraction.Structure;
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
    }
}
using Norns.Destiny.Abstraction.Structure;
using System;

namespace Norns.Destiny.AOT.Structure
{
    public static class AotSymbolExtensions
    {
        public static AccessibilityInfo ConvertToStructure(this Type type)
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
    }
}
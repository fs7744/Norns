using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Structure;

namespace Norns.Destiny.AOT.Structure
{
    public static class JitSymbolExtensions
    {
        public static AccessibilityInfo ConvertToStructure(this Accessibility accessibility)
        {
            switch (accessibility)
            {
                case Accessibility.Private:
                    return AccessibilityInfo.Private;

                case Accessibility.ProtectedAndInternal:
                    return AccessibilityInfo.ProtectedAndInternal;

                case Accessibility.Protected:
                    return AccessibilityInfo.Protected;

                case Accessibility.Internal:
                    return AccessibilityInfo.Internal;

                case Accessibility.ProtectedOrInternal:
                    return AccessibilityInfo.ProtectedOrInternal;

                case Accessibility.Public:
                    return AccessibilityInfo.Public;

                default:
                    return AccessibilityInfo.NotApplicable;
            }
        }
    }
}
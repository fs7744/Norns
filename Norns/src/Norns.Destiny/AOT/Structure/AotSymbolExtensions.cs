using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Structure;

namespace Norns.Destiny.AOT.Structure
{
    public static class AotSymbolExtensions
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

        public static VarianceKindInfo ConvertToStructure(this VarianceKind variance)
        {
            switch (variance)
            {
                case VarianceKind.Out:
                    return VarianceKindInfo.Out;

                case VarianceKind.In:
                    return VarianceKindInfo.In;

                default:
                    return VarianceKindInfo.None;
            }
        }

        public static ISymbolInfo ConvertToStructure(this ISymbol symbol)
        {
            switch (symbol)
            {
                case IFieldSymbol f:
                    return new FieldSymbolInfo(f);
                case IMethodSymbol m:
                    return new MethodSymbolInfo(m);
                default:
                    return null;
            }
        }

        public static RefKindInfo ConvertToStructure(this RefKind refKind)
        {
            switch (refKind)
            {
                case RefKind.Ref:
                    return RefKindInfo.Ref;
                case RefKind.In:
                    return RefKindInfo.In;
                case RefKind.Out:
                    return RefKindInfo.Out;
                default:
                    return RefKindInfo.None;
            }
        }
    }
}
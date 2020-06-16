namespace Norns.Destiny.Abstraction.Structure
{
    public static class CommonSymbolExtensions
    {
        public static string ToDisplayString(this AccessibilityInfo accessibility)
        {
            switch (accessibility)
            {
                case AccessibilityInfo.Private:
                    return "private";

                case AccessibilityInfo.ProtectedAndInternal:
                    return "private protected";

                case AccessibilityInfo.Protected:
                    return "protected";

                case AccessibilityInfo.Internal:
                    return "internal";

                case AccessibilityInfo.ProtectedOrInternal:
                    return "protected internal";

                case AccessibilityInfo.Public:
                    return "public";

                default:
                    return string.Empty;
            }
        }
    }
}
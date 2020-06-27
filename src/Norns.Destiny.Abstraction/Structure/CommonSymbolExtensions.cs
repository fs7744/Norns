namespace Norns.Destiny.Abstraction.Structure
{
    public static class CommonSymbolExtensions
    {
        public const string TaskFullName = "System.Threading.Tasks.Task";
        public const string ValueTaskFullName = "System.Threading.Tasks.ValueTask";
        public const string VoidFullName = "void";

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

        public static (bool, bool) GetMethodExtensionInfo(this IMethodSymbolInfo method)
        {
            var returnTypeStr = method.ReturnType?.FullName;
            if (returnTypeStr == null)
            {
                return (false, false);
            }
            else
            {
                var isTask = returnTypeStr.StartsWith(TaskFullName);
                var isValueTask = returnTypeStr.StartsWith(ValueTaskFullName);
                var isAsync = isTask || isValueTask;
                var hasReturnValue = isAsync ? returnTypeStr.EndsWith(">") : returnTypeStr == VoidFullName;
                return (isAsync, hasReturnValue);
            }
        }
    }
}
using Norns.Destiny.Notations;
using System.Linq;

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

        public static string ToDisplayString(this RefKindInfo refKind)
        {
            switch (refKind)
            {
                case RefKindInfo.In:
                    return "in";

                case RefKindInfo.Out:
                    return "out";

                case RefKindInfo.Ref:
                    return "ref";

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
                var hasReturnValue = isAsync ? returnTypeStr.EndsWith(">") : returnTypeStr != VoidFullName;
                return (isAsync, hasReturnValue);
            }
        }

        public static MethodNotation ToNotationDefinition(this IMethodSymbolInfo method)
        {
            var notation = new MethodNotation()
            {
                Accessibility = method.Accessibility,
                ReturnType = method.ReturnType.FullName,
                Name = method.Name
            };
            notation.Parameters.AddRange(method.Parameters.Select(i => new ParameterNotation()
            {
                RefKind = i.RefKind,
                Type = i.Type.FullName,
                Name = i.Name
            }));
            if (method.IsGenericMethod)
            {
                notation.TypeParameters.AddRange(method.TypeParameters.Select(i => new ParameterNotation()
                {
                    Type = i.FullName
                }));
            }
            notation.IsAsync = method.IsAsync;
            return notation;
        }

        public static bool HasAttribute<T>(this ITypeSymbolInfo type)
        {
            return type.GetAttributes().Any(i => i.AttributeType.FullName == typeof(T).FullName);
        }
    }
}
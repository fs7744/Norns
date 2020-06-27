using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.Notations;
using Norns.Destiny.Utils;
using System.Collections.Generic;

namespace Norns.Destiny.AOP.Notations
{
    public class ProxyGeneratorContext : Dictionary<string, object>
    {
        public ProxyGeneratorContext Parent { get; set; }
        public ISymbolInfo Symbol { get; set; }
    }

    public static class ProxyGeneratorContextExtensions
    {
        private const string ReturnValueParameterName = "ReturnValueParameterName";
        private const string ProxyFieldName = "ProxyFieldName";
        private const string CurrentPropertyMethod = "CurrentPropertyMethod";
        private const string CurrentNamespaceNotation = "CurrentNamespaceNotation";
        private const string CurrentClassNotation = "CurrentClassNotation";
        private const string CurrentMethodNotation = "CurrentMethodNotation";

        public static string GetReturnValueParameterName(this ProxyGeneratorContext context)
        {
            if (!context.ContainsKey(ReturnValueParameterName))
            {
                context.Add(ReturnValueParameterName, $"r{RandomUtils.NewName()}");
            }
            return context[ReturnValueParameterName].ToString();
        }

        public static string GetProxyFieldName(this ProxyGeneratorContext context)
        {
            var c = context.Parent ?? context;
            if (!c.ContainsKey(ProxyFieldName))
            {
                c.Add(ProxyFieldName, $"f{RandomUtils.NewName()}");
            }
            return c[ProxyFieldName].ToString();
        }

        public static void SetCurrentPropertyMethod(this ProxyGeneratorContext context, IMethodSymbolInfo method)
        {
            context[CurrentPropertyMethod] = method;
        }

        public static IMethodSymbolInfo GetCurrentPropertyMethod(this ProxyGeneratorContext context)
        {
            return context.TryGetValue(CurrentPropertyMethod, out var data)
                ? data as IMethodSymbolInfo
                : null;
        }

        public static void SetCurrentNamespaceNotation(this ProxyGeneratorContext context, NamespaceNotation @namespace)
        {
            context[CurrentNamespaceNotation] = @namespace;
        }

        public static NamespaceNotation GetCurrentNamespaceNotation(this ProxyGeneratorContext context)
        {
            return context.TryGetValue(CurrentNamespaceNotation, out var data)
                ? data as NamespaceNotation
                : null;
        }

        public static void SetCurrentClassNotation(this ProxyGeneratorContext context, ClassNotation @class)
        {
            context[CurrentClassNotation] = @class;
        }

        public static ClassNotation GetCurrentClassNotation(this ProxyGeneratorContext context)
        {
            return context.TryGetValue(CurrentClassNotation, out var data)
                ? data as ClassNotation
                : null;
        }

        public static void SetCurrentMethodNotation(this ProxyGeneratorContext context, MethodNotation method)
        {
            context[CurrentMethodNotation] = method;
        }

        public static MethodNotation GetCurrentMethodNotation(this ProxyGeneratorContext context)
        {
            return context.TryGetValue(CurrentMethodNotation, out var data)
                ? data as MethodNotation
                : null;
        }
    }
}
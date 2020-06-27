using Norns.Destiny.Abstraction.Structure;
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
    }
}
using Microsoft.CodeAnalysis;
using Norns.DestinyLoom.Symbols;

namespace Norns.DestinyLoom
{
    public class ProxyMethodGeneratorContext
    {
        private const string TaskFullName = "System.Threading.Tasks.Task";
        private const string ValueTaskFullName = "System.Threading.Tasks.ValueTask";

        public ProxyMethodGeneratorContext(IMethodSymbol method, ProxyGeneratorContext context)
        {
            Method = method;
            ClassGeneratorContext = context;
            var returnTypeStr = method.ReturnType.ToDisplayString();
            var isTask = returnTypeStr.StartsWith(TaskFullName);
            var isValueTask = returnTypeStr.StartsWith(ValueTaskFullName);
            IsAsync = isTask || isValueTask;
            IsAsyncValue = IsAsync && returnTypeStr.EndsWith(">");
            if (IsAsyncValue)
            {
                AsyncValueType = returnTypeStr.Substring((isTask ? TaskFullName.Length : ValueTaskFullName.Length) + 1);
                AsyncValueType = AsyncValueType.Substring(0, AsyncValueType.Length - 1);
            }
            HasReturnValue = IsAsync ? IsAsyncValue : !method.ReturnsVoid;
            ReturnValueParameterName = $"r{GuidHelper.NewGuidName()}";
            Accessibility = method.DeclaredAccessibility.ToDisplayString();
        }

        public IMethodSymbol Method { get; }
        public ProxyGeneratorContext ClassGeneratorContext { get; }
        public bool HasReturnValue { get; }
        public string ReturnValueParameterName { get; }
        public string Accessibility { get; }
        public bool IsAsync { get; }
        public bool IsAsyncValue { get; }
        public string AsyncValueType { get; }
        public string ClassName { get; set; }

        public string GetFromDI(string type)
        {
            return ClassGeneratorContext.GetFromDI(type);
        }

        public void AddUsing(string @namespace)
        {
            ClassGeneratorContext.AddUsing(@namespace);
        }
    }
}
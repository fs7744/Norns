using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Structure;

namespace Norns.Destiny.AOT.Structure
{
    public class TypeSymbolInfo : ITypeSymbolInfo
    {
        private readonly ITypeSymbol type;

        public TypeSymbolInfo(ITypeSymbol type)
        {
            this.type = type;
            Accessibility = type.DeclaredAccessibility.ConvertToStructure();
            Namespace = type.ContainingNamespace.ToDisplayString();
            if ((type is INamedTypeSymbol namedType))
            {
                IsGenericType = namedType.IsGenericType;
                IsAbstract = namedType.IsAbstract;
                Arity = namedType.Arity;
            }
        }

        public string Namespace { get; }
        public AccessibilityInfo Accessibility { get; }
        public string Name => type.Name;
        public bool IsStatic => type.IsStatic;
        public bool IsSealed => type.IsSealed;
        public bool IsValueType => type.IsValueType;
        public bool IsGenericType { get; }
        public int Arity { get; }
        public bool IsAbstract { get; }
        public bool IsAnonymousType => type.IsAnonymousType;
    }
}
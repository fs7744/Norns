using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Structure;
using System.Collections.Immutable;
using System.Linq;

namespace Norns.Destiny.AOT.Structure
{
    public class TypeSymbolInfo : ITypeSymbolInfo
    {
        public TypeSymbolInfo(ITypeSymbol type)
        {
            Origin = type;
            RealType = type;
            Accessibility = type.DeclaredAccessibility.ConvertToStructure();
            Namespace = type.ContainingNamespace.ToDisplayString();
            if (type is INamedTypeSymbol namedType)
            {
                IsGenericType = namedType.IsGenericType;
                IsAbstract = namedType.IsAbstract;
                TypeArguments = namedType.TypeArguments.Select(i => new TypeSymbolInfo(i)).ToImmutableArray<ITypeSymbolInfo>();
                TypeParameters = namedType.TypeParameters.Select(i => new TypeParameterSymbolInfo(i)).ToImmutableArray<ITypeParameterSymbolInfo>();
            }
            else
            {
                TypeArguments = ImmutableArray<ITypeSymbolInfo>.Empty;
                TypeParameters = ImmutableArray<ITypeParameterSymbolInfo>.Empty;
            }
        }

        public ITypeSymbol RealType { get; }
        public string Namespace { get; }
        public AccessibilityInfo Accessibility { get; }
        public string Name => RealType.Name;
        public bool IsStatic => RealType.IsStatic;
        public bool IsSealed => RealType.IsSealed;
        public bool IsValueType => RealType.IsValueType;
        public bool IsGenericType { get; }
        public ImmutableArray<ITypeSymbolInfo> TypeArguments { get; }
        public ImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; }
        public bool IsAbstract { get; }
        public bool IsAnonymousType => RealType.IsAnonymousType;
        public object Origin { get; }
        public bool IsClass => RealType.TypeKind == TypeKind.Class;
        public bool IsInterface => RealType.TypeKind == TypeKind.Interface;
        public string FullName => RealType.ToDisplayString();
        public ITypeSymbolInfo BaseType => RealType.BaseType == null ? null : new TypeSymbolInfo(RealType.BaseType);

        public ImmutableArray<ITypeSymbolInfo> GetInterfaces() => RealType.AllInterfaces
            .Select(i => new TypeSymbolInfo(i))
            .ToImmutableArray<ITypeSymbolInfo>();

        public ImmutableArray<ISymbolInfo> GetMembers() => RealType.GetMembers()
            .Select(AotSymbolExtensions.ConvertToStructure)
            .Where(i => i != null)
            .ToImmutableArray();
    }
}
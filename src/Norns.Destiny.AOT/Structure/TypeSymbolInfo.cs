using Microsoft.CodeAnalysis;
using Norns.Destiny.Immutable;
using Norns.Destiny.Structure;
using System;
using System.Linq;

namespace Norns.Skuld.Structure
{
    public class TypeSymbolInfo : ITypeSymbolInfo
    {
        public TypeSymbolInfo(ITypeSymbol type)
        {
            RealType = type;
            Accessibility = type.DeclaredAccessibility.ConvertToStructure();
            Namespace = type.ContainingNamespace?.ToDisplayString();
            if (type is INamedTypeSymbol namedType)
            {
                IsGenericType = namedType.IsGenericType;
                IsAbstract = namedType.IsAbstract;
                TypeArguments = EnumerableExtensions.CreateLazyImmutableArray<ITypeSymbolInfo>(() => namedType.TypeArguments.Select(i => new TypeSymbolInfo(i)));
                TypeParameters = EnumerableExtensions.CreateLazyImmutableArray<ITypeParameterSymbolInfo>(() => namedType.TypeParameters.Select(i => new TypeParameterSymbolInfo(i)));
                if (IsGenericType)
                {
                    genericDefinitionName = new Lazy<string>(() => $"{(RealType.ContainingType == null ? RealType.ContainingNamespace.ToDisplayString() : RealType.ContainingType.ToDisplayString())}.{Name}<{TypeParameters.Skip(1).Select(i => ",").DefaultIfEmpty("").Aggregate((i, j) => i + j)}>");
                }
                else
                {
                    genericDefinitionName = new Lazy<string>(() => string.Empty);
                }
            }
            else
            {
                TypeArguments = EnumerableExtensions.EmptyImmutableArray<ITypeSymbolInfo>();
                TypeParameters = EnumerableExtensions.EmptyImmutableArray<ITypeParameterSymbolInfo>();
            }
            Attributes = EnumerableExtensions.CreateLazyImmutableArray(() => RealType.GetAttributes()
            .Select(AotSymbolExtensions.ConvertToStructure)
            .Where(i => i != null));
            Interfaces = EnumerableExtensions.CreateLazyImmutableArray<ITypeSymbolInfo>(() => RealType.AllInterfaces.Select(i => new TypeSymbolInfo(i)));
            Members = EnumerableExtensions.CreateLazyImmutableArray(() => RealType.GetMembers()
            .Select(AotSymbolExtensions.ConvertToStructure)
            .Where(i => i != null));
        }

        public ITypeSymbol RealType { get; }
        public string Namespace { get; }
        public AccessibilityInfo Accessibility { get; }
        public string Name => RealType.Name;
        public bool IsStatic => RealType.IsStatic;
        public bool IsSealed => RealType.IsSealed;
        public bool IsValueType => RealType.IsValueType;
        public bool IsGenericType { get; }
        public bool IsAbstract { get; }
        public bool IsAnonymousType => RealType.IsAnonymousType;
        public object Origin => RealType;
        public bool IsClass => RealType.TypeKind == TypeKind.Class;
        public bool IsInterface => RealType.TypeKind == TypeKind.Interface;
        public string FullName => RealType.ToDisplayString();
        public ITypeSymbolInfo BaseType => RealType.BaseType == null ? null : new TypeSymbolInfo(RealType.BaseType);
        private readonly Lazy<string> genericDefinitionName;
        public string GenericDefinitionName => genericDefinitionName.Value;
        public IImmutableArray<ITypeSymbolInfo> TypeArguments { get; }
        public IImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; }
        public IImmutableArray<ITypeSymbolInfo> Interfaces { get; }
        public IImmutableArray<ISymbolInfo> Members { get; }
        public IImmutableArray<IAttributeSymbolInfo> Attributes { get; }

        public override bool Equals(object obj)
        {
            if (obj is ITypeSymbolInfo type && type.FullName != null)
            {
                return type.FullName == FullName;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return FullName == null ? Name.GetHashCode() : FullName.GetHashCode();
        }
    }
}
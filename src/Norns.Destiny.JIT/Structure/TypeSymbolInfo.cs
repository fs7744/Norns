using Norns.Destiny.Abstraction.Structure;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace Norns.Destiny.JIT.Structure
{
    public class TypeSymbolInfo : ITypeSymbolInfo
    {
        public TypeSymbolInfo(Type type)
        {
            RealType = type;
            Accessibility = type.ConvertAccessibilityInfo();
            IsStatic = type.IsAbstract && type.IsSealed;
            if (IsGenericType)
            {
                TypeArguments = type.GenericTypeArguments.Select(i => new TypeSymbolInfo(i)).ToImmutableArray<ITypeSymbolInfo>();
                var genericType = (type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition());
                TypeParameters = genericType.GetGenericArguments().Select(i => new TypeParameterSymbolInfo(i)).ToImmutableArray<ITypeParameterSymbolInfo>();
            }
            else
            {
                TypeArguments = ImmutableArray<ITypeSymbolInfo>.Empty;
                TypeParameters = ImmutableArray<ITypeParameterSymbolInfo>.Empty;
            }
        }

        public Type RealType { get; }
        public string Namespace => RealType.Namespace;
        public AccessibilityInfo Accessibility { get; }
        public string Name => RealType.Name;
        public bool IsStatic { get; }
        public bool IsSealed => RealType.IsSealed;
        public bool IsValueType => RealType.IsValueType;
        public bool IsGenericType => RealType.IsGenericType;
        public int Arity { get; }
        public bool IsAbstract => RealType.IsAbstract;

        public bool IsAnonymousType => Attribute.IsDefined(RealType, typeof(CompilerGeneratedAttribute), false)
            && RealType.Name.Contains("AnonymousType")
            && RealType.Name.StartsWith("<>");

        public ImmutableArray<ITypeSymbolInfo> TypeArguments { get; }
        public object Origin => RealType;
        public ImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; }
        public bool IsClass => RealType.IsClass;
        public bool IsInterface => RealType.IsInterface;
        public string FullName => RealType.FullName;
        public ITypeSymbolInfo BaseType => RealType.BaseType == null ? null : new TypeSymbolInfo(RealType.BaseType);
        public string GenericDefinitionName => $"{RealType.Namespace}.{Name}<{TypeParameters.Skip(1).Select(i => ",").Aggregate((i, j) => i + j)}>";

        public ImmutableArray<IAttributeSymbolInfo> GetAttributes() => RealType.GetCustomAttributesData().Select(i => new AttributeSymbolInfo(i)).ToImmutableArray<IAttributeSymbolInfo>();

        public ImmutableArray<ITypeSymbolInfo> GetInterfaces() => RealType.GetInterfaces()
            .Select(i => new TypeSymbolInfo(i))
            .ToImmutableArray<ITypeSymbolInfo>();

        public ImmutableArray<ISymbolInfo> GetMembers() => RealType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Select(JitSymbolExtensions.ConvertToStructure)
            .Where(i => i != null)
            .ToImmutableArray();
    }
}
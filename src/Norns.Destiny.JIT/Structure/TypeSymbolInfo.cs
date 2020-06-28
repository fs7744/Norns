using Norns.Destiny.Abstraction.Structure;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Norns.Destiny.JIT.Structure
{
    public class TypeSymbolInfo : ITypeSymbolInfo
    {
        private static readonly Regex genericParamsNumber = new Regex("`{1}[0-9]{1,}"); 

        public TypeSymbolInfo(Type type)
        {
            RealType = type;
            Accessibility = type.ConvertAccessibilityInfo();
            IsStatic = type.IsAbstract && type.IsSealed;

            if (type.IsValueType && type.Name == "Void")
            {
                FullName = Name = "void";
            }
            else if (type.IsNested)
            {
                Name = RealType.Name.Replace('+', '.');
                FullName = $"{RealType.DeclaringType.FullName}.{Name}";
            }
            else
            {
                Name = RealType.Name;
                FullName = RealType.FullName;
            }

            if (IsGenericType)
            {
                TypeArguments = type.GenericTypeArguments.Select(i => new TypeSymbolInfo(i)).ToImmutableArray<ITypeSymbolInfo>();
                var genericType = (type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition());
                TypeParameters = genericType.GetGenericArguments().Select(i => new TypeParameterSymbolInfo(i)).ToImmutableArray<ITypeParameterSymbolInfo>();
                var genericParams = type.IsGenericTypeDefinition ? TypeParameters.Cast<ITypeSymbolInfo>() : TypeArguments;
                FullName = $"{RealType.Namespace}.{genericParamsNumber.Replace(Name, string.Empty)}<{genericParams.Select(i => i.FullName).InsertSeparator(",").Aggregate((i, j) => i + j)}>";
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
        public string Name { get; } 
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
        public string FullName { get; }
        public ITypeSymbolInfo BaseType => RealType.BaseType == null ? null : new TypeSymbolInfo(RealType.BaseType);
        public string GenericDefinitionName => $"{RealType.Namespace}.{Name}<{TypeParameters.Select(i => string.Empty).InsertSeparator(",").Aggregate((i, j) => i + j)}>";

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
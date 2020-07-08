using Norns.Destiny.Immutable;
using Norns.Destiny.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace Norns.Destiny.RuntimeSymbol
{
    public class TypeSymbolInfo : ITypeSymbolInfo
    {
        private static readonly Regex genericParamsNumber = new Regex("`{1}[0-9]{1,}");

        private void SetInfo(Type type, string name, Func<string> fullName, string @namespace)
        {
            RealType = type;
            Accessibility = type.ConvertAccessibilityInfo();
            IsStatic = type.IsAbstract && type.IsSealed;
            Name = name;
            Namespace = @namespace;
            baseType = new Lazy<ITypeSymbolInfo>(() => RealType.BaseType?.GetSymbolInfo());
            if (type.IsGenericType)
            {
                TypeArguments = EnumerableExtensions.CreateLazyImmutableArray(() => type.GenericTypeArguments.Select(i => i.GetSymbolInfo()));
                TypeParameters = EnumerableExtensions.CreateLazyImmutableArray<ITypeParameterSymbolInfo>(() => (type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition()).GetGenericArguments().Select(i => new TypeParameterSymbolInfo(i)));
                Func<IEnumerable<ITypeSymbolInfo>> genericParams = () => type.IsGenericTypeDefinition ? TypeParameters.Cast<ITypeSymbolInfo>() : TypeArguments;
                Name = genericParamsNumber.Replace(name, string.Empty);
                Func<string> newFullName = () => genericParamsNumber.Replace($"{(type.IsNested ? type.DeclaringType.GetSymbolInfo().FullName : Namespace)}.{Name}", string.Empty);
                genericDefinitionName = new Lazy<string>(() => $"{newFullName()}<{genericParams().Select(i => string.Empty).InsertSeparator(",").Aggregate((i, j) => i + j)}>");
                fullName = () => $"{newFullName()}<{genericParams().Select(i => i.FullName).InsertSeparator(",").Aggregate((i, j) => i + j)}>";
            }
            else
            {
                genericDefinitionName = new Lazy<string>(() => string.Empty);
                TypeArguments = EnumerableExtensions.EmptyImmutableArray<ITypeSymbolInfo>();
                TypeParameters = EnumerableExtensions.EmptyImmutableArray<ITypeParameterSymbolInfo>();
            }
            this.fullName = new Lazy<string>(fullName);
            Attributes = EnumerableExtensions.CreateLazyImmutableArray<IAttributeSymbolInfo>(() => RealType.GetCustomAttributesData().Select(i => new AttributeSymbolInfo(i)));
            Interfaces = EnumerableExtensions.CreateLazyImmutableArray(() => RealType.GetInterfaces().Distinct().Select(i => i.GetSymbolInfo()));
            Members = EnumerableExtensions.CreateLazyImmutableArray(() => RealType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Select(RuntimeSymbolExtensions.ConvertToStructure)
            .Where(i => i != null));
            isAnonymousType = new Lazy<bool>(() => Attribute.IsDefined(RealType, typeof(CompilerGeneratedAttribute), false)
            && RealType.Name.Contains("AnonymousType")
            && RealType.Name.StartsWith("<>"));
        }

        public TypeSymbolInfo(Type type, string fullName, string name)
        {
            SetInfo(type, name, () => fullName, type.Namespace);
        }

        public TypeSymbolInfo(Type type)
        {
            var @namespace = type.Namespace;
            var name = type.Name;
            var newFullName = type.FullName;

            if (type.IsGenericParameter)
            {
                newFullName = name = type.Name;
            }
            else if (type.IsNested)
            {
                name = name.Replace('+', '.');
                newFullName = $"{type.DeclaringType.GetSymbolInfo().FullName}.{name}";
            }
            if (type.IsByRef)
            {
                name = name?.Replace("&", string.Empty);
                newFullName = name;
            }

            SetInfo(type, name, () => newFullName, @namespace);
        }

        public Type RealType { get; private set; }
        public string Namespace { get; private set; }
        public AccessibilityInfo Accessibility { get; private set; }
        public string Name { get; private set; }
        public bool IsStatic { get; private set; }
        public bool IsSealed => RealType.IsSealed || RealType.IsGenericTypeDefinition;
        public bool IsValueType => RealType.IsValueType;
        public bool IsGenericType => RealType.IsGenericType;
        public bool IsAbstract => RealType.IsAbstract;

        private Lazy<bool> isAnonymousType;
        public bool IsAnonymousType => isAnonymousType.Value;

        public object Origin => RealType;
        public bool IsClass => RealType.IsClass;
        public bool IsInterface => RealType.IsInterface;
        private Lazy<string> fullName;
        public string FullName => fullName.Value;

        private Lazy<ITypeSymbolInfo> baseType;
        public ITypeSymbolInfo BaseType => baseType.Value;
        private Lazy<string> genericDefinitionName;
        public string GenericDefinitionName => genericDefinitionName.Value;
        public IImmutableArray<ITypeSymbolInfo> TypeArguments { get; private set; }
        public IImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; private set; }
        public IImmutableArray<ITypeSymbolInfo> Interfaces { get; private set; }
        public IImmutableArray<ISymbolInfo> Members { get; private set; }
        public IImmutableArray<IAttributeSymbolInfo> Attributes { get; private set; }

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
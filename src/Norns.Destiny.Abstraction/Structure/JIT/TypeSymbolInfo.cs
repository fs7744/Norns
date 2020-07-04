using Norns.Destiny.Abstraction.Structure;
using System;
using System.Collections.Generic;
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

        private static readonly Dictionary<string, string> specialTypes = new Dictionary<string, string>()
        {
            { "System.Void", "void" },
            { typeof(object).FullName, "object" },
            { typeof(bool).FullName, "bool" },
            { typeof(char).FullName, "char" },
            { typeof(short).FullName, "short" },
            { typeof(ushort).FullName, "ushort" },
            { typeof(int).FullName, "int" },
            { typeof(uint).FullName, "uint" },
            { typeof(long).FullName, "long" },
            { typeof(ulong).FullName, "ulong" },
            { typeof(decimal).FullName, "decimal" },
            { typeof(float).FullName, "float" },
            { typeof(double).FullName, "double" },
            { typeof(string).FullName, "string" }
        };

        private void SetInfo(Type type, string name, string fullName, string @namespace)
        {
            RealType = type;
            Accessibility = type.ConvertAccessibilityInfo();
            IsStatic = type.IsAbstract && type.IsSealed;
            Name = name;
            FullName = fullName;
            Namespace = @namespace;
        }

        public TypeSymbolInfo(Type type)
        {
            var @namespace = type.Namespace;
            var name = type.Name;
            var fullName = type.FullName;

            if (fullName != null && specialTypes.TryGetValue(fullName, out var specialTypeFullName))
            {
                fullName = specialTypeFullName;
            }
            else if (type.IsGenericParameter)
            {
                fullName = name = type.Name;
            }
            else if (type.IsNested)
            {
                name = name.Replace('+', '.');
                fullName = $"{type.DeclaringType.GetSymbolInfo().FullName}.{name}";
            }

            if (type.IsGenericType)
            {
                TypeArguments = type.GenericTypeArguments.Select(i => i.GetSymbolInfo()).ToImmutableArray<ITypeSymbolInfo>();
                var genericType = (type.IsGenericTypeDefinition ? type : type.GetGenericTypeDefinition());
                TypeParameters = genericType.GetGenericArguments().Select(i => new TypeParameterSymbolInfo(i)).ToImmutableArray<ITypeParameterSymbolInfo>();
                var genericParams = type.IsGenericTypeDefinition ? TypeParameters.Cast<ITypeSymbolInfo>() : TypeArguments;
                name = genericParamsNumber.Replace(name, string.Empty);
                fullName = genericParamsNumber.Replace($"{(type.IsNested ? type.DeclaringType.GetSymbolInfo().FullName : @namespace)}.{name}", string.Empty);
                GenericDefinitionName = $"{fullName}<{genericParams.Select(i => string.Empty).InsertSeparator(",").Aggregate((i, j) => i + j)}>";
                fullName = $"{fullName}<{genericParams.Select(i => i.FullName).InsertSeparator(",").Aggregate((i, j) => i + j)}>";
            }
            else
            {
                TypeArguments = ImmutableArray<ITypeSymbolInfo>.Empty;
                TypeParameters = ImmutableArray<ITypeParameterSymbolInfo>.Empty;
            }
            if (type.IsByRef)
            {
                name = name?.Replace("&", string.Empty);
                fullName = name;
            }

            SetInfo(type, name, fullName, @namespace);
        }

        public Type RealType { get; private set; }
        public string Namespace { get; private set; }
        public AccessibilityInfo Accessibility { get; private set; }
        public string Name { get; private set; }
        public bool IsStatic { get; private set; }
        public bool IsSealed => RealType.IsSealed;
        public bool IsValueType => RealType.IsValueType;
        public bool IsGenericType => RealType.IsGenericType;
        public bool IsAbstract => RealType.IsAbstract;

        public bool IsAnonymousType => Attribute.IsDefined(RealType, typeof(CompilerGeneratedAttribute), false)
            && RealType.Name.Contains("AnonymousType")
            && RealType.Name.StartsWith("<>");

        public ImmutableArray<ITypeSymbolInfo> TypeArguments { get; private set; }
        public object Origin => RealType;
        public ImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; private set; }
        public bool IsClass => RealType.IsClass;
        public bool IsInterface => RealType.IsInterface;
        public string FullName { get; private set; }
        public ITypeSymbolInfo BaseType => RealType.BaseType == null ? null : RealType.BaseType.GetSymbolInfo();
        public string GenericDefinitionName { get; private set; }

        public ImmutableArray<IAttributeSymbolInfo> GetAttributes() => RealType.GetCustomAttributesData().Select(i => new AttributeSymbolInfo(i)).ToImmutableArray<IAttributeSymbolInfo>();

        public ImmutableArray<ITypeSymbolInfo> GetInterfaces() => RealType.GetInterfaces()
            .Select(i => i.GetSymbolInfo())
            .ToImmutableArray();

        public ImmutableArray<ISymbolInfo> GetMembers() => RealType.GetMembers(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Select(JitSymbolExtensions.ConvertToStructure)
            .Where(i => i != null)
            .ToImmutableArray();

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
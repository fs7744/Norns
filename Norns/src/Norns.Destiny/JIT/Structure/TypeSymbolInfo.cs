using Norns.Destiny.Abstraction.Structure;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Norns.Destiny.JIT.Structure
{
    public class TypeSymbolInfo : ITypeSymbolInfo
    {
        public TypeSymbolInfo(Type type)
        {
            Origin = type;
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

        public object Origin { get; }

        public ImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; }
    }
}
using Norns.Destiny.Abstraction.Structure;
using System;
using System.Runtime.CompilerServices;

namespace Norns.Destiny.JIT.Structure
{
    public class TypeSymbolInfo : ITypeSymbolInfo
    {
        public TypeSymbolInfo(Type type)
        {
            RealType = type;
            Accessibility = type.ConvertToStructure();
            IsStatic = type.IsAbstract && type.IsSealed;
            Arity = type.GenericTypeArguments.Length;
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
    }
}
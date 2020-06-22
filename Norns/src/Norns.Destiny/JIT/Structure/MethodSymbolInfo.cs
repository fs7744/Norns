using Norns.Destiny.Abstraction.Structure;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Norns.Destiny.JIT.Structure
{
    public class MethodSymbolInfo : IMethodSymbolInfo
    {
        public MethodSymbolInfo(MethodInfo m)
        {
            RealMethod = m;
            if (IsGenericMethod)
            {
                var generic = (RealMethod.IsGenericMethodDefinition ? RealMethod : RealMethod.GetGenericMethodDefinition());
                TypeParameters = generic.GetGenericArguments().Select(i => new TypeParameterSymbolInfo(i)).ToImmutableArray<ITypeParameterSymbolInfo>();
            }
            else
            {
                TypeParameters = ImmutableArray<ITypeParameterSymbolInfo>.Empty;
            }
            Parameters = RealMethod.GetParameters().Select(i => new ParameterSymbolInfo(i)).ToImmutableArray<IParameterSymbolInfo>();
            ReturnType = new TypeSymbolInfo(RealMethod.ReturnType);
            Accessibility = RealMethod.ConvertAccessibilityInfo();
        }

        public MethodInfo RealMethod { get; }
        public ITypeSymbolInfo ReturnType { get; }
        public bool IsExtensionMethod => RealMethod.CustomAttributes.Any(i => i.AttributeType == typeof(System.Runtime.CompilerServices.ExtensionAttribute));
        public bool IsGenericMethod => RealMethod.IsGenericMethod;
        public ImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; }
        public ImmutableArray<IParameterSymbolInfo> Parameters { get; }
        public object Origin => RealMethod;
        public string Name => RealMethod.Name;
        public bool IsStatic => RealMethod.IsStatic;
        public AccessibilityInfo Accessibility { get; }
        public bool IsSealed => RealMethod.IsFinal;
        public bool IsAbstract => RealMethod.IsAbstract;
        public bool IsOverride => RealMethod.IsVirtual && RealMethod.IsHideBySig;
        public bool IsVirtual => RealMethod.IsVirtual;
        public bool IsNew => !RealMethod.IsVirtual && RealMethod.IsHideBySig;
    }
}
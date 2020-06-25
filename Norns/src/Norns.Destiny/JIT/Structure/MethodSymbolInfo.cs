using Norns.Destiny.Abstraction.Structure;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;

namespace Norns.Destiny.JIT.Structure
{
    public class MethodSymbolInfo : IMethodSymbolInfo
    {
        public MethodSymbolInfo(MethodBase m)
        {
            RealMethod = m;
            if (IsGenericMethod && m is MethodInfo mi)
            {
                var generic = (mi.IsGenericMethodDefinition ? mi : mi.GetGenericMethodDefinition());
                TypeParameters = generic.GetGenericArguments().Select(i => new TypeParameterSymbolInfo(i)).ToImmutableArray<ITypeParameterSymbolInfo>();
            }
            else
            {
                TypeParameters = ImmutableArray<ITypeParameterSymbolInfo>.Empty;
            }
            Parameters = RealMethod.GetParameters().Select(i => new ParameterSymbolInfo(i)).ToImmutableArray<IParameterSymbolInfo>();
            ReturnType = m is MethodInfo mei ? new TypeSymbolInfo(mei.ReturnType) : null;
            Accessibility = RealMethod.ConvertAccessibilityInfo();
            MethodKind = RealMethod.ConvertMethodKindInfo();
        }

        public MethodBase RealMethod { get; }
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
        public bool IsOverride => RealMethod.IsVirtual && (RealMethod.Attributes & MethodAttributes.NewSlot) != MethodAttributes.NewSlot;
        public bool IsVirtual => RealMethod.IsVirtual && !RealMethod.IsAbstract;
        public string FullName => $"{RealMethod.DeclaringType.FullName}.{RealMethod.Name}";
        public MethodKindInfo MethodKind { get; }

        public ImmutableArray<IAttributeSymbolInfo> GetAttributes() => RealMethod
            .GetCustomAttributesData()
            .Select(i => new AttributeSymbolInfo(i))
            .ToImmutableArray<IAttributeSymbolInfo>();
    }
}
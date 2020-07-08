using Norns.Destiny.Immutable;
using Norns.Destiny.Structure;
using System.Linq;
using System.Reflection;

namespace Norns.Destiny.RuntimeSymbol
{
    public class MethodSymbolInfo : IMethodSymbolInfo
    {
        public MethodSymbolInfo(MethodBase m)
        {
            ContainingType = m.DeclaringType.GetSymbolInfo();
            RealMethod = m;
            if (IsGenericMethod && m is MethodInfo mi)
            {
                TypeParameters = EnumerableExtensions.CreateLazyImmutableArray(() => (mi.IsGenericMethodDefinition ? mi : mi.GetGenericMethodDefinition())
                                    .GetGenericArguments().Select(i => new TypeParameterSymbolInfo(i)));
            }
            else
            {
                TypeParameters = EnumerableExtensions.EmptyImmutableArray<ITypeParameterSymbolInfo>();
            }
            Parameters = EnumerableExtensions.CreateLazyImmutableArray(() => RealMethod.GetParameters().Select(i => new ParameterSymbolInfo(i)));
            ReturnType = m is MethodInfo mei ? mei.ReturnType.GetSymbolInfo() : null;
            Accessibility = RealMethod.ConvertAccessibilityInfo();
            MethodKind = RealMethod.ConvertMethodKindInfo();
            var (isAsync, hasReturnValue) = this.GetMethodExtensionInfo();
            IsAsync = isAsync;
            HasReturnValue = hasReturnValue;
            Attributes = EnumerableExtensions.CreateLazyImmutableArray<IAttributeSymbolInfo>(() => RealMethod.GetCustomAttributesData().Select(i => new AttributeSymbolInfo(i)));
        }

        public ITypeSymbolInfo ContainingType { get; }
        public MethodBase RealMethod { get; }
        public ITypeSymbolInfo ReturnType { get; }
        public bool IsExtensionMethod => RealMethod.CustomAttributes.Any(i => i.AttributeType == typeof(System.Runtime.CompilerServices.ExtensionAttribute));
        public bool IsGenericMethod => RealMethod.IsGenericMethod;
        public object Origin => RealMethod;
        public string Name => RealMethod.Name;
        public bool IsStatic => RealMethod.IsStatic;
        public AccessibilityInfo Accessibility { get; }
        public bool IsSealed => RealMethod.IsFinal;
        public bool IsAbstract => RealMethod.IsAbstract;
        public bool IsOverride => RealMethod.IsVirtual && (RealMethod.Attributes & MethodAttributes.NewSlot) != MethodAttributes.NewSlot;
        public bool IsVirtual => RealMethod.IsVirtual && !RealMethod.IsAbstract;
        private string fullName;

        public string FullName
        {
            get
            {
                if (fullName == null)
                {
                    fullName = $"{ReturnType?.FullName} {RealMethod.Name}{(IsGenericMethod ? ("<" + TypeParameters.Select(i => i.FullName).InsertSeparator(",").Aggregate((i, j) => i + j)) + ">" : string.Empty)}({Parameters.Select(i => i.FullName).InsertSeparator(",").DefaultIfEmpty().Aggregate((i, j) => i + j)})";
                }
                return fullName;
            }
        }

        public MethodKindInfo MethodKind { get; }
        public bool IsAsync { get; }
        public bool HasReturnValue { get; }
        public IImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; }
        public IImmutableArray<IParameterSymbolInfo> Parameters { get; }
        public IImmutableArray<IAttributeSymbolInfo> Attributes { get; }
    }
}
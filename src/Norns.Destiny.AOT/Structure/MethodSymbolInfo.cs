using Microsoft.CodeAnalysis;
using Norns.Destiny.Immutable;
using Norns.Destiny.Structure;
using System.Linq;

namespace Norns.Skuld.Structure
{
    public class MethodSymbolInfo : IMethodSymbolInfo
    {
        public MethodSymbolInfo(IMethodSymbol m)
        {
            RealMethod = m;
            Origin = m;
            Accessibility = m.DeclaredAccessibility.ConvertToStructure();
            ReturnType = new TypeSymbolInfo(m.ReturnType);
            MethodKind = m.MethodKind.ConvertToStructure();
            var (isAsync, hasReturnValue) = this.GetMethodExtensionInfo();
            IsAsync = isAsync;
            HasReturnValue = hasReturnValue;
            Attributes = EnumerableExtensions.CreateLazyImmutableArray(() => RealMethod.GetAttributes()
            .Select(AotSymbolExtensions.ConvertToStructure)
            .Where(i => i != null));
            TypeParameters = EnumerableExtensions.CreateLazyImmutableArray<ITypeParameterSymbolInfo>(() => RealMethod.TypeParameters.Select(i => new TypeParameterSymbolInfo(i)));
            Parameters = EnumerableExtensions.CreateLazyImmutableArray<IParameterSymbolInfo>(() => RealMethod.Parameters.Select(i => new ParameterSymbolInfo(i)));
        }

        public IMethodSymbol RealMethod { get; }
        public object Origin { get; }
        public string Name => RealMethod.Name;
        public bool IsStatic => RealMethod.IsStatic;
        public AccessibilityInfo Accessibility { get; }
        public ITypeSymbolInfo ReturnType { get; }
        public bool IsExtensionMethod => RealMethod.IsExtensionMethod;
        public bool IsGenericMethod => RealMethod.IsGenericMethod;
        public bool IsSealed => RealMethod.IsSealed;
        public bool IsAbstract => RealMethod.IsAbstract;
        public bool IsOverride => RealMethod.IsOverride;
        public bool IsVirtual => RealMethod.IsVirtual;
        public string FullName => RealMethod.ToDisplayString();
        public MethodKindInfo MethodKind { get; }
        public bool IsAsync { get; }
        public bool HasReturnValue { get; }
        public IImmutableArray<ITypeParameterSymbolInfo> TypeParameters { get; }
        public IImmutableArray<IParameterSymbolInfo> Parameters { get; }
        public IImmutableArray<IAttributeSymbolInfo> Attributes { get; }
    }
}
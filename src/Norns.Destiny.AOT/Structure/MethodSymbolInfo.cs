using Microsoft.CodeAnalysis;
using Norns.Destiny.Structure;
using System.Collections.Immutable;
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
        }

        public IMethodSymbol RealMethod { get; }
        public object Origin { get; }
        public string Name => RealMethod.Name;
        public bool IsStatic => RealMethod.IsStatic;
        public AccessibilityInfo Accessibility { get; }
        public ITypeSymbolInfo ReturnType { get; }
        public bool IsExtensionMethod => RealMethod.IsExtensionMethod;
        public bool IsGenericMethod => RealMethod.IsGenericMethod;
        public ImmutableArray<ITypeParameterSymbolInfo> TypeParameters => RealMethod.TypeParameters.Select(i => new TypeParameterSymbolInfo(i)).ToImmutableArray<ITypeParameterSymbolInfo>();
        public ImmutableArray<IParameterSymbolInfo> Parameters => RealMethod.Parameters.Select(i => new ParameterSymbolInfo(i)).ToImmutableArray<IParameterSymbolInfo>();
        public bool IsSealed => RealMethod.IsSealed;
        public bool IsAbstract => RealMethod.IsAbstract;
        public bool IsOverride => RealMethod.IsOverride;
        public bool IsVirtual => RealMethod.IsVirtual;
        public string FullName => RealMethod.ToDisplayString();
        public MethodKindInfo MethodKind { get; }
        public bool IsAsync { get; }
        public bool HasReturnValue { get; }

        public ImmutableArray<IAttributeSymbolInfo> GetAttributes() => RealMethod.GetAttributes()
            .Select(AotSymbolExtensions.ConvertToStructure)
            .Where(i => i != null)
            .ToImmutableArray();
    }
}
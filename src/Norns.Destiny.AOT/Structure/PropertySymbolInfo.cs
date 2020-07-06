using Microsoft.CodeAnalysis;
using Norns.Destiny.Immutable;
using Norns.Destiny.Structure;
using System.Linq;

namespace Norns.Skuld.Structure
{
    public class PropertySymbolInfo : IPropertySymbolInfo
    {
        public PropertySymbolInfo(IPropertySymbol p)
        {
            RealProperty = p;
            Type = new TypeSymbolInfo(p.Type);
            Accessibility = p.DeclaredAccessibility.ConvertToStructure();
            GetMethod = CanRead ? new MethodSymbolInfo(p.GetMethod) : null;
            SetMethod = CanWrite ? new MethodSymbolInfo(p.SetMethod) : null;
            Parameters = EnumerableExtensions.CreateLazyImmutableArray<IParameterSymbolInfo>(() => RealProperty.Parameters.Select(i => new ParameterSymbolInfo(i)));
            Attributes = EnumerableExtensions.CreateLazyImmutableArray(() => RealProperty.GetAttributes()
            .Select(AotSymbolExtensions.ConvertToStructure)
            .Where(i => i != null));
        }

        private IPropertySymbol RealProperty { get; }
        public object Origin => RealProperty;
        public string Name => RealProperty.Name;
        public bool IsIndexer => RealProperty.IsIndexer;
        public bool CanWrite => !RealProperty.IsReadOnly;
        public bool CanRead => !RealProperty.IsWriteOnly;
        public ITypeSymbolInfo Type { get; }
        public AccessibilityInfo Accessibility { get; }
        public bool IsStatic => RealProperty.IsStatic;
        public bool IsExtern => RealProperty.IsExtern;
        public bool IsSealed => RealProperty.IsSealed;
        public bool IsAbstract => RealProperty.IsAbstract;
        public bool IsOverride => RealProperty.IsVirtual && RealProperty.IsOverride;
        public bool IsVirtual => RealProperty.IsVirtual;
        public IMethodSymbolInfo GetMethod { get; }
        public IMethodSymbolInfo SetMethod { get; }
        public string FullName => RealProperty.ToDisplayString();
        public IImmutableArray<IParameterSymbolInfo> Parameters { get; }
        public IImmutableArray<IAttributeSymbolInfo> Attributes { get; }
    }
}
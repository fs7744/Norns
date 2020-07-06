using Microsoft.CodeAnalysis;
using Norns.Destiny.Immutable;
using Norns.Destiny.Structure;
using System.Linq;

namespace Norns.Skuld.Structure
{
    public class FieldSymbolInfo : IFieldSymbolInfo
    {
        public FieldSymbolInfo(IFieldSymbol f)
        {
            RealField = f;
            FieldType = new TypeSymbolInfo(f.Type);
            Accessibility = f.DeclaredAccessibility.ConvertToStructure();
            Attributes = EnumerableExtensions.CreateLazyImmutableArray(() => RealField.GetAttributes()
            .Select(AotSymbolExtensions.ConvertToStructure)
            .Where(i => i != null));
        }

        public IFieldSymbol RealField { get; }
        public object Origin => RealField;
        public string Name => RealField.Name;
        public ITypeSymbolInfo FieldType { get; }
        public bool IsConst => RealField.IsConst;
        public bool IsReadOnly => RealField.IsReadOnly;
        public bool IsVolatile => RealField.IsVolatile;
        public bool IsFixedSizeBuffer => RealField.IsFixedSizeBuffer;
        public bool HasConstantValue => RealField.HasConstantValue;
        public object ConstantValue => RealField.ConstantValue;
        public bool IsStatic => RealField.IsStatic;
        public AccessibilityInfo Accessibility { get; }
        public string FullName => RealField.ToDisplayString();
        public IImmutableArray<IAttributeSymbolInfo> Attributes { get; }
    }
}
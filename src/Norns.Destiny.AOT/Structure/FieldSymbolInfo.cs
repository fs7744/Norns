using Microsoft.CodeAnalysis;
using Norns.Destiny.Structure;
using System.Collections.Immutable;
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

        public ImmutableArray<IAttributeSymbolInfo> GetAttributes() => RealField.GetAttributes()
            .Select(AotSymbolExtensions.ConvertToStructure)
            .Where(i => i != null)
            .ToImmutableArray();
    }
}
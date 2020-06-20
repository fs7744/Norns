using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Structure;

namespace Norns.Destiny.AOT.Structure
{
    public class FieldSymbolInfo : IFieldSymbolInfo
    {
        public FieldSymbolInfo(IFieldSymbol f)
        {
            RealField = f;
            Origin = f;
            FieldType = new TypeSymbolInfo(f.Type);
            Accessibility = f.DeclaredAccessibility.ConvertToStructure();
        }

        public IFieldSymbol RealField { get; }
        public object Origin { get; }
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
    }
}
using Norns.Destiny.Abstraction.Structure;
using System.Linq;
using System.Reflection;

namespace Norns.Destiny.JIT.Structure
{
    public class FieldSymbolInfo : IFieldSymbolInfo
    {
        public FieldSymbolInfo(FieldInfo f)
        {
            RealField = f;
            FieldType = new TypeSymbolInfo(f.FieldType);
            Accessibility = f.ConvertAccessibilityInfo();
        }

        public FieldInfo RealField { get; }
        public object Origin => RealField;
        public string Name => RealField.Name;
        public ITypeSymbolInfo FieldType { get; }
        public bool IsConst => RealField.IsLiteral;
        public bool IsReadOnly => RealField.IsInitOnly;
        public bool IsVolatile => RealField.GetRequiredCustomModifiers().Any(i => i == typeof(System.Runtime.CompilerServices.IsVolatile));
        public bool IsFixedSizeBuffer => RealField.IsDefined(typeof(System.Runtime.CompilerServices.FixedBufferAttribute));
        public bool HasConstantValue => (RealField.Attributes & FieldAttributes.HasDefault) == FieldAttributes.HasDefault;
        public object ConstantValue => RealField.GetRawConstantValue();
        public bool IsStatic => RealField.IsStatic;
        public AccessibilityInfo Accessibility { get; }
    }
}
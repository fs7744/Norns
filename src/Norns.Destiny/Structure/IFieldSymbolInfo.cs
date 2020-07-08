using Norns.Destiny.Immutable;

namespace Norns.Destiny.Structure
{
    public interface IFieldSymbolInfo : ISymbolInfo
    {
        ITypeSymbolInfo FieldType { get; }
        bool IsConst { get; }
        bool IsReadOnly { get; }
        bool IsVolatile { get; }
        bool IsFixedSizeBuffer { get; }
        bool HasConstantValue { get; }
        object ConstantValue { get; }
        bool IsStatic { get; }
        AccessibilityInfo Accessibility { get; }
        IImmutableArray<IAttributeSymbolInfo> Attributes { get; }
        ITypeSymbolInfo ContainingType { get; }
    }
}
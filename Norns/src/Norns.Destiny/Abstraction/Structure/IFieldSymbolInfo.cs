namespace Norns.Destiny.Abstraction.Structure
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
    }
}
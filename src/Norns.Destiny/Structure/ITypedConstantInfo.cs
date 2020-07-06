namespace Norns.Destiny.Structure
{
    public interface ITypedConstantInfo
    {
        ITypeSymbolInfo Type { get; }
        object Value { get; }
    }
}
namespace Norns.Destiny.Abstraction.Structure
{
    public interface ITypedConstantInfo
    {
        ITypeSymbolInfo Type { get; }
        object Value { get; }
    }
}
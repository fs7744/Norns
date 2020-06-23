namespace Norns.Destiny.Abstraction.Structure
{
    internal class TypedConstantInfo : INamedConstantInfo
    {
        public ITypeSymbolInfo Type { get; set; }

        public object Value { get; set; }

        public string Name { get; set; }
    }
}
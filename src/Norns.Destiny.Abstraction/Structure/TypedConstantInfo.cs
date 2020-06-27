namespace Norns.Destiny.Abstraction.Structure
{
    public class TypedConstantInfo : INamedConstantInfo
    {
        public ITypeSymbolInfo Type { get; set; }

        public object Value { get; set; }

        public string Name { get; set; }
    }
}
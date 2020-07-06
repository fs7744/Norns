namespace Norns.Destiny.Structure
{
    public class TypedConstantInfo : INamedConstantInfo
    {
        public ITypeSymbolInfo Type { get; set; }

        public object Value { get; set; }

        public string Name { get; set; }
    }
}
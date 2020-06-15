namespace Norns.DestinyLoom.Symbols
{
    public class ParameterSymbol : SymbolLazyCreater
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public ParameterSymbol() : base(null)
        {
            func = () => Symbol.Merge(Symbol.Create(() => Type.ToSymbol().WithBlank()), Symbol.Create(() => Name.ToSymbol()));
        }
    }
}
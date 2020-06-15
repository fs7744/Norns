namespace Norns.DestinyLoom.Symbols
{
    public class FieldSymbol : GenerateSymbolLinkedList
    {
        public string Accessibility { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public FieldSymbol()
        {
            this.AddSymbols(new IGenerateSymbol[]
            {
                Symbol.Create(() => Accessibility.ToSymbol().WithBlank()),
                Symbol.Create(() => Type.ToSymbol().WithBlank()),
                Symbol.Create(() => Name.ToSymbol()),
                Symbol.KeySemicolon
            });
        }
    }
}
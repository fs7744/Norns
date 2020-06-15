namespace Norns.DestinyLoom.Symbols
{
    public class PropertyMethodSymbol : GenerateSymbolLinkedList
    {
        public string Accessibility { get; set; }

        public string Name { get; set; }

        public GenerateSymbolLinkedList Body { get; } = new GenerateSymbolLinkedList();

        public bool IsIndexer { get; set; }

        public PropertyMethodSymbol()
        {
            this.AddSymbols(new IGenerateSymbol[]
            {
                Symbol.Create(() => Accessibility.ToSymbol().WithBlank()),
                Symbol.Create(() => Name.ToSymbol()),
                Symbol.Merge(() => Body.Count == 0,Symbol.KeySemicolon),
                Symbol.Merge(() => Body.Count > 0, Symbol.KeyOpenBrace.WithFullBlank(),Body,Symbol.KeyCloseBrace.WithBlank())
            });
        }
    }
}
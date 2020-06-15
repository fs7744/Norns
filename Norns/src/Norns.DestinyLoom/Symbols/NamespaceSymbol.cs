namespace Norns.DestinyLoom.Symbols
{
    public class NamespaceSymbol : GenerateSymbolLinkedList
    {
        public string Name { get; set; }

        public GenerateSymbolLinkedList Members { get; } = new GenerateSymbolLinkedList();

        public NamespaceSymbol()
        {
            this.AddSymbols(new IGenerateSymbol[] 
            {
                Symbol.KeyNamespace.WithBlank(), 
                Symbol.Create(() => Name.ToSymbol().WithBlank()), 
                Symbol.KeyOpenBrace.WithBlank(), 
                Members, 
                Symbol.KeyCloseBrace.WithBlank()
            });
        }
    }
}
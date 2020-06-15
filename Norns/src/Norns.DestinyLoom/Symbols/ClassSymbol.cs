namespace Norns.DestinyLoom.Symbols
{
    public class ClassSymbol : GenerateSymbolLinkedList
    {
        public string Accessibility { get; set; }

        public string Name { get; set; }

        public GenerateSymbolLinkedList TypeParameters { get; } = new GenerateSymbolLinkedList() { Separator = Symbol.KeyComma };

        public GenerateSymbolLinkedList Inherits { get; } = new GenerateSymbolLinkedList() { Separator = Symbol.KeyComma };

        public GenerateSymbolLinkedList CustomAttributes { get; } = new GenerateSymbolLinkedList() { Separator = Symbol.KeyBlank };

        public GenerateSymbolLinkedList Members { get; } = new GenerateSymbolLinkedList();

        public ClassSymbol()
        {
            this.AddSymbols(new IGenerateSymbol[]
            {
                Symbol.Merge(() => CustomAttributes.Count > 0, Symbol.KeyBlank, CustomAttributes, Symbol.KeyBlank),
                Symbol.Create(() => Accessibility.ToSymbol().WithBlank()),
                Symbol.KeyClass.WithBlank(),
                Symbol.Create(() => Name.ToSymbol()),
                Symbol.Merge(() => TypeParameters.Count > 0, Symbol.KeyOpenAngleBracket, TypeParameters , Symbol.KeyCloseAngleBracket),
                Symbol.Merge(() => Inherits.Count > 0, Symbol.KeyColon.WithFullBlank(), Inherits),
                Symbol.KeyOpenBrace.WithFullBlank(),
                Members,
                Symbol.KeyCloseBrace.WithBlank()
            });
        }
    }
}
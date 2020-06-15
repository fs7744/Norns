using System.Linq;

namespace Norns.DestinyLoom.Symbols
{
    public class MethodSymbol : GenerateSymbolLinkedList
    {
        public string Accessibility { get; set; }

        public string Name { get; set; }

        public string ReturnType { get; set; }

        public GenerateSymbolLinkedList TypeParameters { get; } = new GenerateSymbolLinkedList() { Separator = Symbol.KeyComma };

        public GenerateSymbolLinkedList Parameters { get; } = new GenerateSymbolLinkedList() { Separator = Symbol.KeyComma };

        //public GenerateSymbolLinkedList Inherits { get; } = new GenerateSymbolLinkedList() { Separator = Symbol.KeyComma };

        public GenerateSymbolLinkedList CustomAttributes { get; } = new GenerateSymbolLinkedList() { Separator = Symbol.KeyBlank };

        public GenerateSymbolLinkedList Body { get; } = new GenerateSymbolLinkedList();
        public GenerateSymbolLinkedList Symbols { get; } = new GenerateSymbolLinkedList() { Separator = Symbol.KeyBlank };
        public GenerateSymbolLinkedList Constraints { get; } = new GenerateSymbolLinkedList();
        public IGenerateSymbol CallParameters { get; }

        public MethodSymbol()
        {
            this.AddSymbols(new IGenerateSymbol[]
            {
                Symbol.Merge(() => CustomAttributes.Count > 0, Symbol.KeyBlank, CustomAttributes, Symbol.KeyBlank),
                Symbol.Create(() => Accessibility.ToSymbol().WithBlank()),
                Symbol.Merge(() => Symbols.Count > 0, Symbols, Symbol.KeyBlank),
                Symbol.Create(() => ReturnType.ToSymbol().WithBlank()),
                Symbol.Create(() => Name.ToSymbol()),
                Symbol.Merge(() => TypeParameters.Count > 0, Symbol.KeyOpenAngleBracket, TypeParameters , Symbol.KeyCloseAngleBracket),
                Symbol.KeyOpenParen,
                Symbol.Merge(() => Parameters.Count > 0, Parameters),
                Symbol.KeyCloseParen,
                Constraints,
                Symbol.KeyOpenBrace.WithFullBlank(),
                Body,
                Symbol.KeyCloseBrace.WithBlank()
            }); 
            CallParameters = Symbol.Merge(Symbol.KeyOpenParen, Symbol.Create(() => Symbol.Merge(Parameters.Select(i => i as ParameterSymbol).Where(i => i != null).Select(i => Symbol.CreateParameter(null, i.Name)).ToArray())), Symbol.KeyCloseParen);
        }
    }
}
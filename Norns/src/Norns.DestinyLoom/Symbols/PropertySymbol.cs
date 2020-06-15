using System.Linq;

namespace Norns.DestinyLoom.Symbols
{
    public class PropertySymbol : GenerateSymbolLinkedList
    {
        public string Accessibility { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public PropertyMethodSymbol Getter { get; } = new PropertyMethodSymbol() { Name = "get" };
        public PropertyMethodSymbol Setter { get; } = new PropertyMethodSymbol() { Name = "set" };
        public GenerateSymbolLinkedList Parameters { get; } = new GenerateSymbolLinkedList() { Separator = Symbol.KeyComma };
        public GenerateSymbolLinkedList Symbols { get; } = new GenerateSymbolLinkedList() { Separator = Symbol.KeyBlank };
        public bool IsIndexer { get; set; }
        public bool CanRead { get; set; }
        public bool CanWrite { get; set; }
        public IGenerateSymbol CallIndexerParameters { get; }

        public PropertySymbol()
        {
            this.AddSymbols(new IGenerateSymbol[]
            {
                Symbol.Create(() => Accessibility.ToSymbol().WithBlank()),
                Symbol.Merge(() => Symbols.Count > 0, Symbols, Symbol.KeyBlank),
                Symbol.Create(() => Type.ToSymbol().WithBlank()),
                Symbol.Merge(() => !IsIndexer, Symbol.Create(() => Name.ToSymbol())),
                Symbol.Merge(() => IsIndexer, Symbol.KeyThis, Symbol.KeyOpenBracket, Parameters, Symbol.KeyCloseBracket),
                Symbol.KeyOpenBrace.WithFullBlank(),
                Symbol.Merge(() => CanRead, Getter),
                Symbol.Merge(() => CanWrite, Setter),
                Symbol.KeyCloseBrace.WithBlank()
            });
            CallIndexerParameters = Symbol.Merge(() => IsIndexer, Symbol.KeyOpenBracket, Symbol.Create(() => Symbol.Merge(Symbol.KeyComma, () => true, Parameters.Select(i => i as ParameterSymbol).Where(i => i != null).Select(i => Symbol.CreateParameter(null, i.Name)).ToArray())), Symbol.KeyCloseBracket);
        }

        public void SetGetterAccessibility(string accessibility)
        {
            Getter.Accessibility = accessibility;
            if (Accessibility == Getter.Accessibility)
            {
                Getter.Accessibility = string.Empty;
            }
        }

        public void SetSetterAccessibility(string accessibility)
        {
            Setter.Accessibility = accessibility;
            if (Accessibility == Setter.Accessibility)
            {
                Setter.Accessibility = string.Empty;
            }
        }
    }
}
using System;
using System.Collections.Generic;
using System.Text;

namespace Norns.DestinyLoom.Symbols
{
    public class UsingSymbol : GenerateSymbolLinkedList
    {
        public string Name { get; set; }

        public UsingSymbol()
        {
            this.AddSymbols(new IGenerateSymbol[]
            {
                Symbol.KeyUsing.WithBlank(),
                Symbol.Create(() => Name.ToSymbol()),
                Symbol.KeySemicolon
            });
        }
    }
}

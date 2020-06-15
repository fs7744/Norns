using System;
using System.Linq;
using System.Text;

namespace Norns.DestinyLoom.Symbols
{
    public class SymbolMerger : IGenerateSymbol
    {
        private readonly Func<bool> canMerge;
        private readonly IGenerateSymbol[] symbols;

        public IGenerateSymbol Separator { get; set; }

        public SymbolMerger(Func<bool> canMerge, params IGenerateSymbol[] symbols)
        {
            this.canMerge = canMerge;
            this.symbols = symbols;
        }

        public void Generate(StringBuilder sb)
        {
            if (canMerge() && symbols.Length > 0)
            {
                symbols[0].Generate(sb);
                foreach (var item in symbols.Skip(1))
                {
                    Separator?.Generate(sb);
                    item.Generate(sb);
                }
            }
        }
    }
}
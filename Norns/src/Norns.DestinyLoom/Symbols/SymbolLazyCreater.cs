using System;
using System.Text;

namespace Norns.DestinyLoom.Symbols
{
    public class SymbolLazyCreater : IGenerateSymbol
    {
        protected Func<IGenerateSymbol> func;

        public SymbolLazyCreater(Func<IGenerateSymbol> func)
        {
            this.func = func;
        }

        void IGenerateSymbol.Generate(StringBuilder sb)
        {
            func()?.Generate(sb);
        }
    }
}
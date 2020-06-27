using Norns.Destiny.Abstraction.Structure;
using System.Collections.Generic;

namespace Norns.Destiny.Abstraction.Coder
{
    public interface ISymbolSource
    {
        IEnumerable<ITypeSymbolInfo> GetTypes();
    }
}
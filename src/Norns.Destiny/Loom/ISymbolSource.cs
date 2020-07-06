using Norns.Destiny.Structure;
using System.Collections.Generic;

namespace Norns.Destiny.Loom
{
    public interface ISymbolSource
    {
        IEnumerable<ITypeSymbolInfo> GetTypes();
    }
}
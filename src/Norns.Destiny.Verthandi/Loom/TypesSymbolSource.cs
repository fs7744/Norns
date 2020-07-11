using Norns.Destiny.Loom;
using Norns.Destiny.RuntimeSymbol;
using Norns.Destiny.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Verthandi.Loom
{
    public class TypesSymbolSource : ISymbolSource
    {
        private readonly Type[] types;

        public TypesSymbolSource(params Type[] types)
        {
            this.types = types;
        }

        public IEnumerable<ITypeSymbolInfo> GetTypes()
        {
            return types.Select(i => (i.IsGenericType ? i.GetGenericTypeDefinition() : i).GetSymbolInfo());
        }
    }
}
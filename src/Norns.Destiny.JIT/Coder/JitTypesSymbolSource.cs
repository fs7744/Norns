using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.JIT.Coder
{
    public class JitTypesSymbolSource : ISymbolSource
    {
        private readonly Type[] types;

        public JitTypesSymbolSource(params Type[] types)
        {
            this.types = types;
        }

        public IEnumerable<ITypeSymbolInfo> GetTypes()
        {
            return types.Select(i => (i.IsGenericType ? i.GetGenericTypeDefinition() : i).GetSymbolInfo());
        }
    }
}
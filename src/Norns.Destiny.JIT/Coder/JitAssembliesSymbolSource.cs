using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.JIT.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Norns.Destiny.JIT.Coder
{
    public class JitAssembliesSymbolSource : ISymbolSource
    {
        private readonly Assembly[] assemblies;
        private readonly Func<ITypeSymbolInfo, bool> filter;

        public JitAssembliesSymbolSource(Assembly[] assemblies, Func<ITypeSymbolInfo, bool> filter)
        {
            this.assemblies = assemblies;
            this.filter = filter;
        }

        public IEnumerable<ITypeSymbolInfo> GetTypes()
        {
            return assemblies.SelectMany(i => i.GetTypes().Select(j => j.IsGenericType ? j.GetGenericTypeDefinition() : j))
                .Distinct()
                 .Select(i => new TypeSymbolInfo(i))
                 .Where(filter);
        }
    }
}
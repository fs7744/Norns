using Norns.Destiny.Loom;
using Norns.Destiny.RuntimeSymbol;
using Norns.Destiny.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Norns.Verthandi.Loom
{
    public class AssembliesSymbolSource : ISymbolSource
    {
        private readonly Assembly[] assemblies;
        private readonly Func<ITypeSymbolInfo, bool> filter;

        public AssembliesSymbolSource(Assembly[] assemblies, Func<ITypeSymbolInfo, bool> filter)
        {
            this.assemblies = assemblies;
            this.filter = filter;
        }

        public IEnumerable<ITypeSymbolInfo> GetTypes()
        {
            return assemblies.SelectMany(i => i.GetTypes().Select(j => j.IsGenericType ? j.GetGenericTypeDefinition() : j))
                .Distinct()
                 .Select(i => i.GetSymbolInfo())
                 .Where(filter);
        }
    }
}
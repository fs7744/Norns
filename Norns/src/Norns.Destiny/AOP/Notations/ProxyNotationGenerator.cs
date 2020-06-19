using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.Notations;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.AOP.Notations
{
    public class ProxyNotationGenerator : INotationGenerator
    {
        private readonly IEnumerable<IInterceptorGenerator> generators;

        public ProxyNotationGenerator(IEnumerable<IInterceptorGenerator> generators)
        {
            this.generators = generators;
        }

        public INotation GenerateNotations(ISymbolSource source)
        {
            return source.GetTypes()
                .Select(GenerateProxyType)
                .Aggregate(Notation.Combine);
        }

        private INotation GenerateProxyType(ITypeSymbolInfo type)
        {
            throw new NotImplementedException();
        }
    }
}
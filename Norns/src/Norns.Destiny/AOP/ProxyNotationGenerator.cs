using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.Notations;
using System;
using System.Linq;

namespace Norns.Destiny.AOP
{
    public class ProxyNotationGenerator : INotationGenerator
    {
        public INotation GenerateNotations(ISymbolSource source)
        {
            return source.GetTypes()
                .Select(GenerateProxyType)
                .Aggregate(Notation.Combine);
        }

        private INotation GenerateProxyType(ITypeSymbolInfo arg)
        {
            throw new NotImplementedException();
        }
    }
}
using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.Notations;
using System;
using System.Linq;

namespace Norns.Destiny.AOP
{
    public class ProxyNotationGenerator : INotationGenerator
    {
        private readonly Func<ITypeSymbolInfo, bool> canAop;

        public ProxyNotationGenerator(Func<ITypeSymbolInfo, bool> canAop)
        {
            this.canAop = canAop;
        }

        public INotation GenerateNotations(ISymbolSource source)
        {
            return source.GetTypes()
                .Where(canAop)
                .Select(GenerateProxyType)
                .Aggregate(Notation.Combine);
        }

        private INotation GenerateProxyType(ITypeSymbolInfo arg)
        {
            throw new NotImplementedException();
        }
    }
}
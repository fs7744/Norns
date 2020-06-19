using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Notations;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.AOP
{
    public class NotationGenerators : INotationGenerator
    {
        private readonly IEnumerable<INotationGenerator> generators;

        public NotationGenerators(IEnumerable<INotationGenerator> generators)
        {
            this.generators = generators;
        }

        public INotation GenerateNotations(ISymbolSource source)
        {
            return generators
                .Select(i => i.GenerateNotations(source))
                .Aggregate(Notation.Combine);
        }
    }
}
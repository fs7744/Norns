using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.Notations;
using System.Linq;

namespace Norns.Destiny.AOP.Notations
{
    public abstract class AbstractNotationGenerator : INotationGenerator
    {
        public abstract bool Filter(ITypeSymbolInfo type);

        public virtual INotation GenerateNotations(ISymbolSource source)
        {
            return source.GetTypes().Where(Filter).Select(CreateImplement).Combine();
        }

        public abstract INotation CreateImplement(ITypeSymbolInfo type);
    }
}
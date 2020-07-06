using Norns.Destiny.Loom;
using Norns.Destiny.Notations;
using Norns.Destiny.Structure;
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
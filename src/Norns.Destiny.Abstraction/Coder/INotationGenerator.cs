using Norns.Destiny.Notations;

namespace Norns.Destiny.Abstraction.Coder
{
    public interface INotationGenerator
    {
        INotation GenerateNotations(ISymbolSource source);
    }
}
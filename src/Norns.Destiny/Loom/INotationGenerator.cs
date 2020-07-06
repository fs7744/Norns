using Norns.Destiny.Notations;

namespace Norns.Destiny.Loom
{
    public interface INotationGenerator
    {
        INotation GenerateNotations(ISymbolSource source);
    }
}
using System.Collections.Generic;

namespace Norns.Destiny.Notations
{
    public interface IFormatter<in T> where T : INotation
    {
        IEnumerable<INotation> Format(T value);
    }
}
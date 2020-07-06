using System.Collections.Generic;

namespace Norns.Destiny.Immutable
{
    public interface IImmutableArray<out T> : IReadOnlyCollection<T>
    {
        bool IsEmpty { get; }
    }
}
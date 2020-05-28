using System.Collections.Generic;

namespace Norns.DependencyInjection
{
    internal static class LinkedListExtensions
    {
        internal static LinkedList<T> Add<T>(this LinkedList<T> linkedList, T value)
        {
            if (linkedList.Count == 0)
            {
                linkedList.AddFirst(value);
            }
            else
            {
                linkedList.AddAfter(linkedList.Last, value);
            }

            return linkedList;
        }
    }
}
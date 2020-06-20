using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.Notations
{
    public static class Notation
    {
        public static INotation Combine(INotation x, INotation y)
        {
            return new ActionNotation(sb =>
            {
                x.Record(sb);
                y.Record(sb);
            });
        }

        public static INotation Combine(this IEnumerable<INotation> notations)
        {
            return notations.DefaultIfEmpty(ConstNotations.Nothing).Aggregate(Combine);
        }

        #region ToNotation

        public static INotation ToNotation(this string value)
        {
            return new StringNotation(value);
        }

        #endregion ToNotation
    }
}
using System.Collections.Generic;

namespace Norns.Destiny.Notations
{
    public static class Notation
    {
        public static INotation Combine(this IEnumerable<INotation> notations)
        {
            return new ActionNotation(sb =>
            {
                foreach (var item in notations)
                {
                    item.Record(sb);
                }
            });
        }

        public static IEnumerable<INotation> InsertSeparator(this IEnumerable<INotation> notations, INotation separator)
        {
            var enumerator = notations.GetEnumerator();
            if (enumerator.MoveNext())
            {
                yield return enumerator.Current;
            }
            while (enumerator.MoveNext())
            {
                yield return separator;
                yield return enumerator.Current;
            }
        }

        public static IEnumerable<INotation> InsertComma(this IEnumerable<INotation> notations)
        {
            return InsertSeparator(notations, ConstNotations.Comma);
        }

        public static IEnumerable<INotation> InsertBlank(this IEnumerable<INotation> notations)
        {
            return InsertSeparator(notations, ConstNotations.Blank);
        }

        #region ToNotation

        public static INotation ToNotation(this string value)
        {
            return new StringNotation(value);
        }

        #endregion ToNotation
    }
}
using System.Collections.Generic;
using System.Linq;

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

        public static INotation ToCallParameters(this IEnumerable<ParameterNotation> values)
        {
            return values.Select(i => i.ToCallParameter()).InsertComma().Combine();
        }

        public static IEnumerable<INotation> ToNotations(this IEnumerable<string> values)
        {
            return values.Select(i => i.ToNotation());
        }

        public static IEnumerable<INotation> Create(params string[] values)
        {
            return ToNotations(values);
        }

        #endregion ToNotation
    }
}
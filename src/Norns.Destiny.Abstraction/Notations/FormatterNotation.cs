using System.Text;

namespace Norns.Destiny.Notations
{
    public class FormatterNotation<T> : INotation where T : INotation
    {
        private readonly T origin;
        private readonly IFormatter<T> formatter;

        public FormatterNotation(T origin, IFormatter<T> formatter)
        {
            this.origin = origin;
            this.formatter = formatter;
        }

        public void Record(StringBuilder sb)
        {
            formatter.Format(origin).Combine().Record(sb);
        }
    }
}
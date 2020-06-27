using System.Text;

namespace Norns.Destiny.Notations
{
    public class StringNotation : INotation
    {
        private readonly string value;

        public StringNotation(string value)
        {
            this.value = value;
        }

        public void Record(StringBuilder sb)
        {
            sb.Append(value ?? string.Empty);
        }
    }
}
using System.Text;

namespace Norns.DestinyLoom.Symbols
{
    public class StringSymbol : IGenerateSymbol
    {
        public StringSymbol(string value)
        {
            Value = value;
        }

        public string Value { get; }

        public void Generate(StringBuilder sb)
        {
            if (!string.IsNullOrEmpty(Value))
            {
                sb.Append(Value);
            }
        }
    }
}
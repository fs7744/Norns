using System.Collections.Generic;

namespace Norns.Destiny.Notations
{
    public class IndexerPropertyNotation : PropertyNotation
    {
        public List<ParameterNotation> Parameters { get; } = new List<ParameterNotation>();

        public override INotation GetName()
        {
            return new INotation[]
            {
                "this".ToNotation(),
                ConstNotations.OpenBracket,
                Parameters.InsertComma().Combine(),
                ConstNotations.CloseBracket
            }.Combine();
        }
    }
}
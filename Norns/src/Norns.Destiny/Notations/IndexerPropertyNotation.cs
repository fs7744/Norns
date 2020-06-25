using System.Collections.Generic;

namespace Norns.Destiny.Notations
{
    public class IndexerPropertyNotation : PropertyNotation
    {
        public List<INotation> Parameters { get; } = new List<INotation>();

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
using Norns.Destiny.Structure;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.Notations
{
    public class ConstructorNotation : MethodNotation
    {
        public List<ParameterNotation> BaseParameters { get; } = new List<ParameterNotation>();
        public bool HasBase { get; set; }

        public override IEnumerable<INotation> GetMembers()
        {
            yield return CustomAttributes.InsertBlank().Combine();
            yield return Accessibility.ToDisplayString().ToNotation();
            yield return ConstNotations.Blank;
            yield return Name.ToNotation();
            yield return ConstNotations.OpenParen;
            yield return BaseParameters.Union(Parameters).InsertComma().Combine();
            yield return ConstNotations.CloseParen;
            if (HasBase)
            {
                yield return ConstNotations.Colon;
                yield return ConstNotations.Base;
                yield return ConstNotations.OpenParen;
                yield return BaseParameters.Select(i => i.ToCallParameter()).InsertComma().Combine();
                yield return ConstNotations.CloseParen;
            }
            yield return ConstNotations.OpenBrace;
            yield return Body.Combine();
            yield return ConstNotations.CloseBrace;
        }
    }
}
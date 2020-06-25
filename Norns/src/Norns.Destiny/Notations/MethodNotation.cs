using Norns.Destiny.Abstraction.Structure;
using System.Collections.Generic;

namespace Norns.Destiny.Notations
{
    public class MethodNotation : MembersNotation
    {
        public List<INotation> CustomAttributes { get; } = new List<INotation>();
        public AccessibilityInfo Accessibility { get; set; }
        public string ReturnType { get; set; }
        public string Name { get; set; }
        public List<INotation> TypeParameters { get; } = new List<INotation>();
        public List<INotation> Constraints { get; } = new List<INotation>();
        public List<INotation> Parameters { get; } = new List<INotation>();
        public List<INotation> Body { get; } = new List<INotation>();

        public override IEnumerable<INotation> GetMembers()
        {
            yield return CustomAttributes.InsertBlank().Combine();
            yield return Accessibility.ToDisplayString().ToNotation();
            yield return ConstNotations.Blank;
            yield return ReturnType.ToNotation();
            yield return ConstNotations.Blank;
            yield return Name.ToNotation();
            if (TypeParameters.Count > 0)
            {
                yield return ConstNotations.OpenAngleBracket;
                yield return TypeParameters.InsertComma().Combine();
                yield return ConstNotations.CloseAngleBracket;
            }
            yield return ConstNotations.OpenParen;
            yield return Parameters.InsertComma().Combine();
            yield return ConstNotations.CloseParen;
            yield return Constraints.InsertBlank().Combine();
            yield return ConstNotations.OpenBrace;
            yield return Body.Combine();
            yield return ConstNotations.CloseBrace;
        }
    }
}
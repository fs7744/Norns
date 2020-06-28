using Norns.Destiny.Abstraction.Structure;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.Notations
{
    public class ClassNotation : MembersNotation
    {
        public List<INotation> CustomAttributes { get; } = new List<INotation>();
        public AccessibilityInfo Accessibility { get; set; }
        public string Name { get; set; }
        public List<TypeParameterNotation> TypeParameters { get; } = new List<TypeParameterNotation>();
        public List<INotation> Inherits { get; } = new List<INotation>();
        public List<INotation> Members { get; } = new List<INotation>();

        public override IEnumerable<INotation> GetMembers()
        {
            yield return CustomAttributes.InsertBlank().Combine();
            yield return Accessibility.ToDisplayString().ToNotation();
            yield return ConstNotations.Blank;
            yield return "class".ToNotation();
            yield return ConstNotations.Blank;
            yield return Name.ToNotation();
            if (TypeParameters.Count > 0)
            {
                yield return ConstNotations.OpenAngleBracket;
                yield return TypeParameters.Select(i => i.ToOnlyTypeDefinitionNotation()).InsertComma().Combine();
                yield return ConstNotations.CloseAngleBracket;
            }
            if (Inherits.Count > 0)
            {
                yield return ConstNotations.Colon;
                yield return Inherits.InsertBlank().Combine();
            }
            yield return TypeParameters.Select(i => i.ToConstantNotation()).InsertBlank().Combine();
            yield return ConstNotations.Blank;
            yield return ConstNotations.OpenBrace;
            yield return Members.Combine();
            yield return ConstNotations.CloseBrace;
        }
    }
}
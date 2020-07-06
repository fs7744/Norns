using Norns.Destiny.Structure;
using System.Collections.Generic;

namespace Norns.Destiny.Notations
{
    public class FieldNotation : MembersNotation
    {
        public bool IsFromDI { get; set; }
        public AccessibilityInfo Accessibility { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public override IEnumerable<INotation> GetMembers()
        {
            yield return Accessibility.ToDisplayString().ToNotation();
            yield return ConstNotations.Blank;
            yield return Type.ToNotation();
            yield return ConstNotations.Blank;
            yield return Name.ToNotation();
            yield return ConstNotations.Semicolon;
        }
    }
}
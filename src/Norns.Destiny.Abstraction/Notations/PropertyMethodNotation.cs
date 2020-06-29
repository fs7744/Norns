using Norns.Destiny.Abstraction.Structure;
using System.Collections.Generic;

namespace Norns.Destiny.Notations
{
    public class PropertyMethodNotation : MembersNotation
    {
        public AccessibilityInfo Accessibility { get; set; }
        public string Name { get; set; }
        public List<INotation> Body { get; } = new List<INotation>();

        public override IEnumerable<INotation> GetMembers()
        {
            yield return Accessibility.ToDisplayString().ToNotation();
            yield return ConstNotations.Blank;
            yield return Name.ToNotation();
            if (Body.Count > 0)
            {
                yield return ConstNotations.OpenBrace;
                yield return Body.Combine();
                yield return ConstNotations.CloseBrace;
            }
            else
            {
                yield return ConstNotations.Semicolon;
            }
        }

        public PropertyMethodNotation MakeAccessibilitySafe(AccessibilityInfo accessibility)
        {
            if (accessibility <= Accessibility)
            {
                Accessibility = AccessibilityInfo.NotApplicable;
            }
            return this;
        }

        public static PropertyMethodNotation Create(bool isGetter)
        {
            return new PropertyMethodNotation() { Name = isGetter ? "get" : "set" };
        }
    }
}
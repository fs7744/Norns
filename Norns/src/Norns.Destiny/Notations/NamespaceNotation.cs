using System.Collections.Generic;

namespace Norns.Destiny.Notations
{
    public class NamespaceNotation : MembersNotation
    {
        public string Name { get; set; }
        public List<INotation> Usings { get; } = new List<INotation>();
        public List<INotation> Members { get; } = new List<INotation>();

        public override IEnumerable<INotation> GetMembers()
        {
            yield return "namespace".ToNotation();
            yield return ConstNotations.Blank;
            yield return new StringNotation(Name);
            yield return ConstNotations.Blank;
            yield return ConstNotations.OpenBrace;
            yield return Usings.Combine();
            yield return Members.Combine();
            yield return ConstNotations.CloseBrace;
        }
    }
}
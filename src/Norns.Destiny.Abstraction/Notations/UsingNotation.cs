using System.Collections.Generic;

namespace Norns.Destiny.Notations
{
    public class UsingNotation : MembersNotation
    {
        public string Namespace { get; set; }

        public override IEnumerable<INotation> GetMembers()
        {
            yield return "using".ToNotation();
            yield return ConstNotations.Blank;
            yield return Namespace.ToNotation();
            yield return ConstNotations.Semicolon;
        }
    }
}
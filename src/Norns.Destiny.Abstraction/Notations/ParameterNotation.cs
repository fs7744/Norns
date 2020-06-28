using Norns.Destiny.Abstraction.Structure;
using System.Collections.Generic;

namespace Norns.Destiny.Notations
{
    public class ParameterNotation : MembersNotation
    {
        public RefKindInfo RefKind { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }

        public override IEnumerable<INotation> GetMembers()
        {
            if (RefKind != RefKindInfo.None)
            {
                yield return RefKind.ToDisplayString().ToNotation();
                yield return ConstNotations.Blank;
            }
            yield return Type.ToNotation();
            if (!string.IsNullOrEmpty(Name))
            {
                yield return ConstNotations.Blank;
                yield return Name.ToNotation();
            }
        }

        public FormatterNotation<ParameterNotation> ToCallParameter()
        {
            return new FormatterNotation<ParameterNotation>(this, new CallParameterFormatter());
        }
    }

    public class CallParameterFormatter : IFormatter<ParameterNotation>
    {
        public IEnumerable<INotation> Format(ParameterNotation value)
        {
            yield return value.Name.ToNotation();
        }
    }
}
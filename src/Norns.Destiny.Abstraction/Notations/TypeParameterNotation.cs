using System.Collections.Generic;

namespace Norns.Destiny.Notations
{
    public class TypeParameterNotation : ParameterNotation
    {
        public List<string> Constants { get; } = new List<string>();

        public FormatterNotation<TypeParameterNotation> ToConstantNotation()
        {
            return new FormatterNotation<TypeParameterNotation>(this, new ConstantFormatterNotation());
        }

        public FormatterNotation<TypeParameterNotation> ToOnlyTypeDefinitionNotation()
        {
            return new FormatterNotation<TypeParameterNotation>(this, new OnlyTypeDefinitionNotation());
        }
    }

    public class ConstantFormatterNotation : IFormatter<TypeParameterNotation>
    {
        public IEnumerable<INotation> Format(TypeParameterNotation value)
        {
            yield return ConstNotations.Where;
            yield return ConstNotations.Blank;
            yield return value.Type.ToNotation();
            yield return ConstNotations.Blank;
            yield return ConstNotations.Colon;
            yield return ConstNotations.Blank;
            yield return value.Constants.ToNotations().InsertComma().Combine();
        }
    }

    public class OnlyTypeDefinitionNotation : IFormatter<TypeParameterNotation>
    {
        public IEnumerable<INotation> Format(TypeParameterNotation value)
        {
            yield return value.Type.ToNotation();
        }
    }
}
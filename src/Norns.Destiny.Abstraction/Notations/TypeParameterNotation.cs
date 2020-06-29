using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.Notations
{
    public class TypeParameterNotation : ParameterNotation
    {
        public List<INotation> Constants { get; } = new List<INotation>();

        public FormatterNotation<TypeParameterNotation> ToConstantNotation(bool isOverride)
        {
            return new FormatterNotation<TypeParameterNotation>(this, new ConstantFormatterNotation(isOverride));
        }

        public FormatterNotation<TypeParameterNotation> ToOnlyTypeDefinitionNotation()
        {
            return new FormatterNotation<TypeParameterNotation>(this, new OnlyTypeDefinitionNotation());
        }
    }

    public class ConstantFormatterNotation : IFormatter<TypeParameterNotation>
    {
        private readonly bool isOverride;

        public ConstantFormatterNotation(bool isOverride)
        {
            this.isOverride = isOverride;
        }

        public IEnumerable<INotation> Format(TypeParameterNotation value)
        {
            var constants = value.Constants.Where(i => !isOverride || i == ConstNotations.Class || i == ConstNotations.Struct).ToArray();
            if (constants.Length > 0)
            {
                yield return ConstNotations.Where;
                yield return ConstNotations.Blank;
                yield return value.Type.ToNotation();
                yield return ConstNotations.Blank;
                yield return ConstNotations.Colon;
                yield return ConstNotations.Blank;
                yield return constants.InsertComma().Combine();
            }
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
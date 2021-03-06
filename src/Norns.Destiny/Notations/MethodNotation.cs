﻿using Norns.Destiny.Structure;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.Notations
{
    public class MethodNotation : MembersNotation
    {
        public List<INotation> CustomAttributes { get; } = new List<INotation>();
        public AccessibilityInfo Accessibility { get; set; }
        public bool IsAsync { get; set; }
        public bool IsOverride { get; set; }
        public string ReturnType { get; set; }
        public string Name { get; set; }
        public List<TypeParameterNotation> TypeParameters { get; } = new List<TypeParameterNotation>();
        public List<ParameterNotation> Parameters { get; } = new List<ParameterNotation>();
        public List<INotation> Body { get; } = new List<INotation>();

        public override IEnumerable<INotation> GetMembers()
        {
            yield return CustomAttributes.InsertBlank().Combine();
            yield return Accessibility.ToDisplayString().ToNotation();
            if (IsAsync)
            {
                yield return ConstNotations.Blank;
                yield return ConstNotations.Async;
            }
            if (IsOverride)
            {
                yield return ConstNotations.Blank;
                yield return ConstNotations.Override;
            }
            yield return ConstNotations.Blank;
            yield return ReturnType.ToNotation();
            yield return ConstNotations.Blank;
            yield return Name.ToNotation();
            if (TypeParameters.Count > 0)
            {
                yield return ConstNotations.OpenAngleBracket;
                yield return TypeParameters.Select(i => i.ToOnlyTypeDefinitionNotation()).InsertComma().Combine();
                yield return ConstNotations.CloseAngleBracket;
            }
            yield return ConstNotations.OpenParen;
            yield return Parameters.InsertComma().Combine();
            yield return ConstNotations.CloseParen;
            yield return TypeParameters.Select(i => i.ToConstantNotation(IsOverride)).InsertBlank().Combine();
            yield return ConstNotations.OpenBrace;
            yield return Body.Combine();
            yield return ConstNotations.CloseBrace;
        }
    }
}
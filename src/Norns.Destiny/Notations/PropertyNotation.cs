﻿using Norns.Destiny.Structure;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.Notations
{
    public class PropertyNotation : MembersNotation
    {
        public List<INotation> CustomAttributes { get; } = new List<INotation>();
        public AccessibilityInfo Accessibility { get; set; }
        public bool IsOverride { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        public List<PropertyMethodNotation> Accessers { get; } = new List<PropertyMethodNotation>();

        public override IEnumerable<INotation> GetMembers()
        {
            yield return CustomAttributes.InsertBlank().Combine();
            yield return Accessibility.ToDisplayString().ToNotation();
            if (IsOverride)
            {
                yield return ConstNotations.Blank;
                yield return ConstNotations.Override;
            }
            yield return ConstNotations.Blank;
            yield return Type.ToNotation();
            yield return ConstNotations.Blank;
            yield return GetName();
            yield return ConstNotations.OpenBrace;
            yield return Accessers.Select(i => i.MakeAccessibilitySafe(Accessibility)).Combine();
            yield return ConstNotations.CloseBrace;
        }

        public virtual INotation GetName()
        {
            return Name.ToNotation();
        }
    }
}
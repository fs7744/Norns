using Norns.Destiny.Abstraction.Structure;
using System;

namespace Norns.Destiny.AOT.Structure
{
    public class TypeSymbolInfo : ITypeSymbolInfo
    {
        private readonly Type type;

        public TypeSymbolInfo(Type type)
        {
            this.type = type;
            Accessibility = type.ConvertToStructure();
        }

        public AccessibilityInfo Accessibility { get; }
    }
}
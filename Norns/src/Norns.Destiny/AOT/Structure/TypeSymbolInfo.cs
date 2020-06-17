using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Structure;

namespace Norns.Destiny.AOT.Structure
{
    public class TypeSymbolInfo : ITypeSymbolInfo
    {
        private readonly ITypeSymbol type;

        public TypeSymbolInfo(ITypeSymbol type)
        {
            this.type = type;
            Accessibility = type.DeclaredAccessibility.ConvertToStructure();
        }

        public AccessibilityInfo Accessibility { get; }
    }
}
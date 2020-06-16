using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Structure;

namespace Norns.Destiny.JIT.Structure
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
using Microsoft.CodeAnalysis;
using Norns.Destiny.Loom;
using Norns.Destiny.Structure;
using Norns.Skuld.Structure;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Skuld.Loom
{
    public class SyntaxNodeSymbolSource : ISymbolSource
    {
        private readonly IEnumerable<SyntaxNode> syntaxNodes;
        private readonly Compilation compilation;

        public SyntaxNodeSymbolSource(IEnumerable<SyntaxNode> syntaxNodes, SourceGeneratorContext context)
        {
            this.syntaxNodes = syntaxNodes;
            compilation = context.Compilation;
        }

        public IEnumerable<ITypeSymbolInfo> GetTypes()
        {
            return syntaxNodes
                 .Select(i => compilation.GetSemanticModel(i.SyntaxTree).GetDeclaredSymbol(i))
                 .Where(i => i is INamedTypeSymbol)
                 .Select(i => new TypeSymbolInfo(i as INamedTypeSymbol));
        }
    }
}
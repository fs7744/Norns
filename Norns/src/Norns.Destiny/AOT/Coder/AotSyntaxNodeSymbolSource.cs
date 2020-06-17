using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOT.Structure;
using System.Collections.Generic;

namespace Norns.Destiny.AOT.Coder
{
    public class AotSyntaxNodeSymbolSource : ISymbolSource
    {
        private readonly IEnumerable<SyntaxNode> syntaxNodes;
        private readonly Compilation compilation;

        public AotSyntaxNodeSymbolSource(IEnumerable<SyntaxNode> syntaxNodes, SourceGeneratorContext context)
        {
            this.syntaxNodes = syntaxNodes;
            this.compilation = context.Compilation;
        }

        public IEnumerable<ITypeSymbolInfo> GetTypes()
        {
            foreach (var node in syntaxNodes)
            {
                var model = compilation.GetSemanticModel(node.SyntaxTree);
                if (model.GetDeclaredSymbol(node) is INamedTypeSymbol type)
                {
                    yield return new TypeSymbolInfo(type);
                }
            }
        }
    }
}
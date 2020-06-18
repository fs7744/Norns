using Microsoft.CodeAnalysis;
using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOT.Structure;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.AOT.Coder
{
    public class AotSyntaxNodeSymbolSource : ISymbolSource
    {
        private readonly IEnumerable<SyntaxNode> syntaxNodes;
        private readonly Func<ITypeSymbolInfo, bool> filter;
        private readonly Compilation compilation;

        public AotSyntaxNodeSymbolSource(IEnumerable<SyntaxNode> syntaxNodes, SourceGeneratorContext context, Func<ITypeSymbolInfo, bool> filter)
        {
            this.syntaxNodes = syntaxNodes;
            this.filter = filter;
            compilation = context.Compilation;
        }

        public IEnumerable<ITypeSymbolInfo> GetTypes()
        {
            return syntaxNodes
                 .Select(i => compilation.GetSemanticModel(i.SyntaxTree).GetDeclaredSymbol(i))
                 .Where(i => i is INamedTypeSymbol)
                 .Select(i => new TypeSymbolInfo(i as INamedTypeSymbol))
                 .Where(filter);
        }
    }
}
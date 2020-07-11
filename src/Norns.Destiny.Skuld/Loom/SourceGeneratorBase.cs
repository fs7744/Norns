using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Norns.Destiny.Loom;
using Norns.Destiny.Notations;
using Norns.Destiny.Structure;
using Norns.Destiny.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Norns.Skuld.Loom
{
    public abstract class SourceGeneratorBase : ISourceGenerator
    {
        protected abstract IEnumerable<INotationGenerator> CreateNotationGenerators();

        protected virtual bool FilterSyntaxNode(SyntaxNode syntaxNode)
        {
            return syntaxNode is TypeDeclarationSyntax;
        }

        protected virtual ISymbolSource CreateGenerateSymbolSource(IEnumerable<SyntaxNode> syntaxNodes, SourceGeneratorContext context)
        {
            return new SyntaxNodeSymbolSource(syntaxNodes, context);
        }

        protected virtual SourceText CreateSourceText(INotation notation)
        {
            var sb = new StringBuilder();
            notation.Record(sb);
            return SourceText.From(sb.ToString(), Encoding.UTF8);
        }

        public void Execute(SourceGeneratorContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;
            var file = RandomUtils.NewCSFileName();
            try
            {
                var source = CreateGenerateSymbolSource(receiver.SyntaxNodes, context);
                var notations = CreateNotationGenerators()
                    .Select(i => i.GenerateNotations(source))
                    .Combine();
                context.AddSource(file, CreateSourceText(notations));
            }
            catch (Exception ex)
            {
                context.ReportDiagnostic(Diagnostic.Create(new DiagnosticDescriptor("n001", ex.ToString(), ex.ToString(), "Norns.AOT.Generate", DiagnosticSeverity.Warning, true), Location.Create(file, TextSpan.FromBounds(0, 0), new LinePositionSpan())));
            }
        }

        public void Initialize(InitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver(FilterSyntaxNode));
        }

        internal class SyntaxReceiver : ISyntaxReceiver
        {
            private readonly Func<SyntaxNode, bool> filter;

            internal List<SyntaxNode> SyntaxNodes { get; } = new List<SyntaxNode>();

            public SyntaxReceiver(Func<SyntaxNode, bool> filter)
            {
                this.filter = filter;
            }

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                if (filter(syntaxNode))
                {
                    SyntaxNodes.Add(syntaxNode);
                }
            }
        }
    }
}
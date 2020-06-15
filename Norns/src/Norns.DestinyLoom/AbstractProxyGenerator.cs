using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Norns.DestinyLoom.Symbols;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Norns.DestinyLoom
{
    public abstract class AbstractProxyGenerator : ISourceGenerator
    {
        protected Encoding Encoding { get; set; } = Encoding.UTF8;

        public void Execute(SourceGeneratorContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;
            var @namespace = $"Norns.Destiny.Proxy{GuidHelper.NewGuidName()}";
            var compilation = context.Compilation;
            var interceptors = FindInterceptorGenerators().ToArray();
            var symbols = Symbol.Merge(FindProxyClassGenerators(interceptors).SelectMany(generator => GenerateProxyClass(generator, receiver.CandidateTypes, context, compilation, @namespace)).ToArray());
            var sb = new StringBuilder();
            symbols.Generate(sb);
            context.AddSource($"{@namespace}.cs", SourceText.From(sb.ToString(), Encoding));
        }

        private IEnumerable<IGenerateSymbol> GenerateProxyClass(AbstractProxyClassGenerator generator, IEnumerable<TypeDeclarationSyntax> typeSyntaxs,
            SourceGeneratorContext context, Compilation compilation, string @namespace)
        {
            return typeSyntaxs.Select(typeSyntax =>
            {
                var model = compilation.GetSemanticModel(typeSyntax.SyntaxTree);
                if (model.GetDeclaredSymbol(typeSyntax) is INamedTypeSymbol @type
                    && CanProxy(@type)
                    && generator.CanProxy(@type))
                {
                    var proxyGeneratorContext = new ProxyGeneratorContext(@type, context, @namespace);
                    return generator.Generate(proxyGeneratorContext);
                }
                else
                {
                    return null;
                }
            }).Where(i => i != null);
        }

        public void Initialize(InitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public virtual bool CanProxy(INamedTypeSymbol @type)
        {
            return !@type.IsStatic
                && !@type.IsValueType
                && !@type.IsAnonymousType
                && !@type.IsComImport
                && !@type.IsSealed
                && !@type.IsTupleType
                && !@type.IsUnmanagedType;
        }

        public abstract IEnumerable<IInterceptorGenerator> FindInterceptorGenerators();

        public abstract IEnumerable<AbstractProxyClassGenerator> FindProxyClassGenerators(IInterceptorGenerator[] interceptors);

        internal class SyntaxReceiver : ISyntaxReceiver
        {
            internal List<TypeDeclarationSyntax> CandidateTypes { get; } = new List<TypeDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                switch (syntaxNode)
                {
                    case TypeDeclarationSyntax @type:
                        CandidateTypes.Add(@type);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
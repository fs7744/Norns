using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using Norns.DestinyLoom.Symbols;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Norns.DestinyLoom
{
    public class ProxyGeneratorContext
    {
        public ProxyGeneratorContext(INamedTypeSymbol typeSymbol, SourceGeneratorContext context, string @namespace)
        {
            Type = typeSymbol;
            SourceGeneratorContext = context;
            Namespace = @namespace;
            ProxyFieldName = $"proxy{GuidHelper.NewGuidName()}";
        }

        public INamedTypeSymbol Type { get; }
        public SourceGeneratorContext SourceGeneratorContext { get; }
        public string Namespace { get; }
        public string ProxyFieldName { get; }
    }

    public class ProxyMethodGeneratorContext
    {
        private const string TaskFullName = "System.Threading.Tasks.Task";
        private const string ValueTaskFullName = "System.Threading.Tasks.ValueTask";

        public ProxyMethodGeneratorContext(IMethodSymbol method, ProxyGeneratorContext context)
        {
            Method = method;
            ClassGeneratorContext = context;
            var returnTypeStr = method.ReturnType.ToDisplayString();
            var isTask = returnTypeStr.StartsWith(TaskFullName);
            var isValueTask = returnTypeStr.StartsWith(ValueTaskFullName);
            IsAsync = isTask || isValueTask;
            IsAsyncValue = IsAsync && returnTypeStr.EndsWith(">");
            if (IsAsyncValue)
            {
                AsyncValueType = returnTypeStr.Substring((isTask ? TaskFullName.Length : ValueTaskFullName.Length) + 1);
                AsyncValueType = AsyncValueType.Substring(0, AsyncValueType.Length - 1);
            }
            HasReturnValue = IsAsync ? IsAsyncValue : !method.ReturnsVoid;
            ReturnValueParameterName = $"r{GuidHelper.NewGuidName()}";
        }

        public IMethodSymbol Method { get; }
        public ProxyGeneratorContext ClassGeneratorContext { get; }
        public bool HasReturnValue { get; }
        public string ReturnValueParameterName { get; }
        public bool IsAsync { get; }
        public bool IsAsyncValue { get; }
        public string AsyncValueType { get; }
        public string ClassName { get; set; }
    }

    public class ProxyPropertyGeneratorContext
    {
        public ProxyPropertyGeneratorContext(IPropertySymbol property, ProxyGeneratorContext context)
        {
            Property = property;
            ClassGeneratorContext = context;
        }

        public IPropertySymbol Property { get; }
        public ProxyGeneratorContext ClassGeneratorContext { get; }
    }

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
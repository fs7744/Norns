using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Norns.DestinyLoom
{
    public class ProxyGeneratorContext
    {
        public ProxyGeneratorContext(INamedTypeSymbol typeSymbol, SourceGeneratorContext context, StringBuilder sb, string @namespace)
        {
            Type = typeSymbol;
            SourceGeneratorContext = context;
            Content = sb;
            Namespace = @namespace;
            ProxyFieldName = $"proxy{GuidHelper.NewGuidName()}";
        }

        public INamedTypeSymbol Type { get; }
        public SourceGeneratorContext SourceGeneratorContext { get; }
        public StringBuilder Content { get; }
        public string Namespace { get; }
        public string ProxyFieldName { get; }
    }

    public class ProxyMethodGeneratorContext
    {
        const string TaskFullName = "System.Threading.Tasks.Task";
        const string ValueTaskFullName = "System.Threading.Tasks.ValueTask";

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
            var sb = new StringBuilder();
            var @namespace = $"Norns.Destiny.Proxy{GuidHelper.NewGuidName()}";
            var compilation = context.Compilation;
            var interceptors = FindInterceptorGenerators().ToArray();
            foreach (var generator in FindProxyClassGenerators(interceptors))
            {
                GenerateProxyClass(generator, receiver.CandidateTypes, context, compilation, sb, @namespace);
            }
            context.AddSource($"{@namespace}.cs", SourceText.From(sb.ToString(), Encoding));
        }

        private void GenerateProxyClass(AbstractProxyClassGenerator generator, IEnumerable<TypeDeclarationSyntax> typeSyntaxs,
            SourceGeneratorContext context, Compilation compilation, StringBuilder sb, string @namespace)
        {
            foreach (var typeSyntax in typeSyntaxs)
            {
                var model = compilation.GetSemanticModel(typeSyntax.SyntaxTree);
                if (model.GetDeclaredSymbol(typeSyntax) is INamedTypeSymbol @type
                    && !@type.IsStatic
                    && !@type.IsValueType
                    && !@type.IsAnonymousType
                    && !@type.IsComImport
                    && !@type.IsSealed
                    && !@type.IsTupleType
                    && !@type.IsUnmanagedType
                    && CanProxy(@type)
                    && generator.CanProxy(@type))
                {
                    var proxyGeneratorContext = new ProxyGeneratorContext(@type, context, sb, @namespace);
                    generator.Generate(proxyGeneratorContext);
                }
            }
        }

        public void Initialize(InitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public abstract bool CanProxy(INamedTypeSymbol @type);

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
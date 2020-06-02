using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Norns.DestinyLoom
{
    public class ProxyGeneratorContext
    {
        public ProxyGeneratorContext(INamedTypeSymbol typeSymbol, SourceGeneratorContext context)
        {
            Type = typeSymbol;
            SourceGeneratorContext = context;
            ProxyFieldName = $"proxy{GuidHelper.NewGuidName()}";
        }

        public INamedTypeSymbol Type { get; }
        public SourceGeneratorContext SourceGeneratorContext { get; }
        public string ProxyFieldName { get; }
    }

    public class ProxyMethodGeneratorContext
    {
        public ProxyMethodGeneratorContext(IMethodSymbol method, ProxyGeneratorContext context)
        {
            Method = method;
            ClassGeneratorContext = context;
            HasReturnValue = method.ReturnType.ToDisplayString() != "void";
            if (HasReturnValue)
            {
                ReturnValueParameterName = $"r{GuidHelper.NewGuidName()}";
            }
        }

        public IMethodSymbol Method { get; }
        public ProxyGeneratorContext ClassGeneratorContext { get; }
        public bool HasReturnValue { get; }
        public string ReturnValueParameterName { get; }
    }

    public abstract class AbstractProxyGenerator : ISourceGenerator
    {
        protected Encoding Encoding { get; set; } = Encoding.UTF8;

        public void Execute(SourceGeneratorContext context)
        {
            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
                return;
            var compilation = context.Compilation;
            var interceptors = FindInterceptorGenerators().ToArray();
            GenerateProxyClass(new InterfaceProxyClassGenerator(interceptors), receiver.CandidateInterfaces, context, compilation);
            GenerateProxyClass(new ClassProxyClassGenerator(interceptors), receiver.CandidateClasses, context, compilation);
        }

        private void GenerateProxyClass(AbstractProxyClassGenerator generator, IEnumerable<TypeDeclarationSyntax> typeSyntaxs, SourceGeneratorContext context, Compilation compilation)
        {
            foreach (var typeSyntax in typeSyntaxs)
            {
                var model = compilation.GetSemanticModel(typeSyntax.SyntaxTree);
                if (model.GetDeclaredSymbol(typeSyntax) is INamedTypeSymbol @type
                    && !@type.IsStatic
                    && !@type.IsValueType
                    && CanProxy(@type))
                {
                    var proxyGeneratorContext = new ProxyGeneratorContext(@type, context);
                    var (fileName, content) = generator.Generate(proxyGeneratorContext);
                    if (!string.IsNullOrWhiteSpace(content))
                    {
                        context.AddSource(fileName, SourceText.From(content, Encoding));
                    }
                }
            }
        }

        public void Initialize(InitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        public abstract bool CanProxy(INamedTypeSymbol @type);

        public abstract IEnumerable<IInterceptorGenerator> FindInterceptorGenerators();

        internal class SyntaxReceiver : ISyntaxReceiver
        {
            internal List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();
            internal List<InterfaceDeclarationSyntax> CandidateInterfaces { get; } = new List<InterfaceDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                switch (syntaxNode)
                {
                    case ClassDeclarationSyntax @class:
                        CandidateClasses.Add(@class);
                        break;

                    case InterfaceDeclarationSyntax @interface:
                        CandidateInterfaces.Add(@interface);
                        break;

                    default:
                        break;
                }
            }
        }
    }

    //    [Generator]
    //    public class ProxyGenerator : ISourceGenerator
    //    {
    //        public void Execute(SourceGeneratorContext context)
    //        {
    //            if (!(context.SyntaxReceiver is SyntaxReceiver receiver))
    //                return;

    //            CSharpParseOptions options = (context.Compilation as CSharpCompilation).SyntaxTrees[0].Options as CSharpParseOptions;
    //            Compilation compilation = context.Compilation;
    //            INamedTypeSymbol attributeSymbol = compilation.GetTypeByMetadataName("Norns.Fate.ProxyAttribute");
    //            foreach (var item in receiver.CandidateClass)
    //            {
    //                SemanticModel model = compilation.GetSemanticModel(item.SyntaxTree);
    //                var classSymbol = model.GetDeclaredSymbol(item) as INamedTypeSymbol;
    //                if (classSymbol.GetAttributes().Any(ad => ad.AttributeClass.Equals(attributeSymbol, SymbolEqualityComparer.Default)))
    //                {
    //                    string classSource = ProcessClass(classSymbol);
    //                    context.AddSource($"{classSymbol.Name}_proxy.cs", SourceText.From(classSource, Encoding.UTF8));
    //                }
    //            }
    //        }

    //        private string ProcessClass(INamedTypeSymbol classSymbol)
    //        {
    //            string namespaceName = classSymbol.ContainingNamespace.ToDisplayString();

    //            StringBuilder source = new StringBuilder($@"
    //using System;
    //using {namespaceName};
    //{string.Join(" ", classSymbol.Interfaces.Select(i => $"[assembly: Norns.Adapters.DependencyInjection.Attributes.ProxyMapping(typeof({i.Name}), typeof({classSymbol.Name}), typeof({classSymbol.Name}Proxy))]"))}
    //namespace {namespaceName}
    //{{
    //    public class {classSymbol.Name}Proxy");
    //            var interfaces = string.Join(",", classSymbol.Interfaces.Select(i => i.Name));
    //            if (!string.IsNullOrEmpty(interfaces))
    //            {
    //                source.Append(": ");
    //                source.Append(interfaces);
    //            }
    //            source.Append($@" {{
    //private {classSymbol.Name} _proxy;
    //public {classSymbol.Name}Proxy({classSymbol.Name} proxy)
    //{{
    //    _proxy = proxy;
    //}}
    //");
    //            foreach (var member in classSymbol.GetMembers())
    //            {
    //                switch (member)
    //                {
    //                    case IMethodSymbol method when !method.IsImplicitlyDeclared :
    //                        source.Append($@"public {method.ReturnType} {method.Name}({string.Join(",", method.Parameters.Select(p => $"{p.Type} {p.Name}"))})
    //{{
    //    var r = default({method.ReturnType});
    //try {{
    //     {method.Parameters[0].Name} = {method.Parameters[0].Name} + 3;
    //     Console.WriteLine(""+3"");
    //     if(r > 5) return r;
    //     r = _proxy.{method.Name}({string.Join(",", method.Parameters.Select(p => $"{p.Name}"))});
    //     Console.WriteLine(""+1"");
    //    r = r * -1;
    //     Console.WriteLine(""* -1"");
    //}} finally {{
    //     Console.WriteLine(""= {{0}}"", r); }}
    //return r;
    //}}");
    //                        break;
    //                    default:
    //                        break;
    //                }
    //            }

    //            source.Append("} } ");
    //            return source.ToString();
    //        }

    //        public void Initialize(InitializationContext context)
    //        {
    //            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
    //        }

    //        class SyntaxReceiver : ISyntaxReceiver
    //        {
    //            public List<ClassDeclarationSyntax> CandidateClass { get; } = new List<ClassDeclarationSyntax>();

    //            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    //            {
    //                if(InterfaceDeclarationSyntax )

    //                if (syntaxNode is ClassDeclarationSyntax classDeclarationSyntax
    //                    && classDeclarationSyntax.AttributeLists.Count > 0)
    //                {
    //                    CandidateClass.Add(classDeclarationSyntax);
    //                }
    //            }
    //        }
    //    }
}
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
            internal List<ClassDeclarationSyntax> CandidateClasses { get; } = new List<ClassDeclarationSyntax>();
            internal List<InterfaceDeclarationSyntax> CandidateInterfaces { get; } = new List<InterfaceDeclarationSyntax>();

            public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
            {
                switch (syntaxNode)
                { 
                    case TypeDeclarationSyntax @type:
                        CandidateTypes.Add(@type);
                        break;

                    //case ClassDeclarationSyntax @class:
                    //    CandidateClasses.Add(@class);
                    //    break;

                    //case InterfaceDeclarationSyntax @interface:
                    //    CandidateInterfaces.Add(@interface);
                    //    break;

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
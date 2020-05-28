using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Formatting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Norns.AOP.Codegen
{
    public class ProxyCodeGenerator
    {

        public string SrcDirectory { get; set; }
        public string OutputDirectory { get; set; }

        public void Generate()
        {
            var files = CSharpFileUtils.ListAllCSharpFile(SrcDirectory);
            var walker = new ProxyCSharpSyntaxWalker();
            foreach (var file in files)
            {
                walker.Visit(CSharpSyntaxTree.ParseText(File.ReadAllText(file)).GetRoot());
            }

            var unit = SyntaxFactory.CompilationUnit().AddMembers(SyntaxFactory.NamespaceDeclaration(SyntaxFactory.ParseName("Norns.Aop.Proxy"))
                .AddMembers(walker.ClassDeclarations.Select(i => CreateProxySyntaxTree(i)).ToArray()));
            var content = Formatter.Format(unit, new AdhocWorkspace()).ToFullString();
            File.WriteAllText(Path.Combine(OutputDirectory ,"NornsProxy.cs"), content);
        }

        private MemberDeclarationSyntax CreateProxySyntaxTree(ClassDeclarationSyntax classDec)
        {
            var baseList = SyntaxFactory.BaseList(
                    SyntaxFactory.SeparatedList<BaseTypeSyntax>(
                        new SimpleBaseTypeSyntax[]
                        {
                            SyntaxFactory.SimpleBaseType(SyntaxFactory.ParseTypeName(classDec.Identifier.ToString()))
                        }));

            var members = SyntaxFactory.List(new MemberDeclarationSyntax[]
            {
                CreateProperty(nameof(IServiceProvider), "ProxyServiceProvider", true),
                CreateProperty("IInterceptDelegateBuilder", "ProxyInterceptBuilder", true)
            }
            .Union(CreateConstructors(classDec))
            .Union(CreateMethods(classDec)));

            var proxyDec = SyntaxFactory.ClassDeclaration($"{classDec.Identifier.ToString()}_Proxy")
                .WithAttributeLists(classDec.AttributeLists)
                .WithModifiers(classDec.Modifiers)
                .WithBaseList(baseList)
                .WithMembers(members);
            return proxyDec;
        }

        private IEnumerable<MemberDeclarationSyntax> CreateMethods(ClassDeclarationSyntax classDec)
        {
            return classDec.Members.Where(i => i is MethodDeclarationSyntax)
                .SelectMany(i => CreateMethod(i as MethodDeclarationSyntax))
                .Where(i => i != null);
        }

        private IEnumerable<MemberDeclarationSyntax> CreateMethod(MethodDeclarationSyntax dec)
        {
            var result = new List<MemberDeclarationSyntax>();
            if (dec.Modifiers.Any(i => i.Text == "public")
                && dec.Modifiers.Any(i => i.Text == "virtual" || i.Text == "override")
                && !dec.Modifiers.Any(i => i.Text == "static")
                && !dec.AttributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToString().EndsWith("NoIntercept")))
            {
                var methodRealName = $"{dec.Identifier.ToString()}_Real";
                var methodParameterTypes = string.Join(",", dec.ParameterList.Parameters.Select(i => $"typeof({i.Type.ToString()})"));
                var field = SyntaxFactory.FieldDeclaration(SyntaxFactory.VariableDeclaration(SyntaxFactory.ParseTypeName("MethodBase"),
                    SyntaxFactory.SingletonSeparatedList(SyntaxFactory.VariableDeclarator(methodRealName)
                    .WithInitializer(SyntaxFactory.EqualsValueClause(SyntaxFactory.ParseExpression(
                        $"this.GetType().GetMethod({dec.Identifier.ToString()}, new Type[] {{{ methodParameterTypes}}})"))))))
                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword));

                result.Add(field);
                var statements = new List<StatementSyntax>()
                {
                    SyntaxFactory.ParseStatement("var context = new InterceptContext() { ServiceProvider = ProxyServiceProvider };\r\n")
                };
                statements.Add(SyntaxFactory.ParseStatement($"context.ServiceMethod = {methodRealName};\r\n"));
                var methodParameters = string.Join(",", dec.ParameterList.Parameters.Select(i => $"{i.Identifier.ToString()}"));
                statements.Add(SyntaxFactory.ParseStatement($"context.Parameters = new object[] {{{methodParameters}}};\r\n"));
                var index = 0;
                var parameters = string.Join(",", dec.ParameterList.Parameters.Select(i => $"({i.Type.ToString()})context.Parameters[{index++}]"));
                if (dec.Modifiers.Any(i => i.Text == "async"))
                {
                    statements.Add(SyntaxFactory.ParseStatement($"await ProxyInterceptBuilder.BuildAsyncInterceptDelegate({methodRealName}, async c => {{ c.Result = await base.{dec.Identifier.ToString()}({parameters});}})(context);\r\n"));
                }
                else if (dec.ReturnType.ToString() == "void")
                {
                    statements.Add(SyntaxFactory.ParseStatement($"ProxyInterceptBuilder.BuildInterceptDelegate({methodRealName}, c => {{ base.{dec.Identifier.ToString()}({parameters});}})(context);\r\n"));
                }
                else
                {
                    statements.Add(SyntaxFactory.ParseStatement($"ProxyInterceptBuilder.BuildInterceptDelegate({methodRealName}, c => {{ c.Result = base.{dec.Identifier.ToString()}({parameters});}})(context);\r\n"));
                }
                var returnType = dec.ReturnType.ToString();
                if (returnType != "void" && returnType != "Task")
                {
                    var rt = returnType;
                    if (returnType.StartsWith("Task<"))
                    {
                        rt = rt.Replace("Task<", "");
                        rt = rt.Remove(rt.Length - 1);
                    }
                    if(returnType.StartsWith("ValueTask<"))
                    {
                        rt = rt.Replace("ValueTask<", "");
                        rt = rt.Remove(rt.Length - 1);
                    }
                    statements.Add(SyntaxFactory.ParseStatement($"return ({rt})context.Result;"));
                }
                var proxyMethod = SyntaxFactory.MethodDeclaration(dec.ReturnType, dec.Identifier.ToString())
                    .WithAttributeLists(dec.AttributeLists)
                    .WithParameterList(dec.ParameterList)
                    .WithModifiers(SyntaxFactory.TokenList(dec.Modifiers.Select(i => i.Text == "virtual" ? SyntaxFactory.Token(SyntaxKind.OverrideKeyword) : i)))
                    .WithBody(SyntaxFactory.Block(statements.ToArray()));
                result.Add(proxyMethod);
            }
            return result;
        }

        private IEnumerable<MemberDeclarationSyntax> CreateConstructors(ClassDeclarationSyntax classDec)
        {
            return classDec.Members.Where(i => i is ConstructorDeclarationSyntax)
                .Select(i => CreateConstructor(i as ConstructorDeclarationSyntax))
                .Where(i => i != null);
        }

        private MemberDeclarationSyntax CreateConstructor(ConstructorDeclarationSyntax dec)
        {
            if (dec.Modifiers.Any(i => i.Text == "public")
               && !dec.Modifiers.Any(i => i.Text == "static"))
            {
                var argumentList = SyntaxFactory.ArgumentList(SyntaxFactory.SeparatedList(
                    dec.ParameterList.Parameters.Select(i => SyntaxFactory.Argument(SyntaxFactory.ParseExpression(i.Identifier.ToString())))));
                return SyntaxFactory.ConstructorDeclaration(dec.Identifier.ToString())
                    .WithAttributeLists(dec.AttributeLists)
                    .WithParameterList(dec.ParameterList)
                    .WithModifiers(dec.Modifiers)
                    .WithInitializer(
                        SyntaxFactory.ConstructorInitializer(SyntaxKind.BaseConstructorInitializer, argumentList))
                        .WithBody(SyntaxFactory.Block());
            }
            else
            {
                return null;
            }
        }

        private PropertyDeclarationSyntax CreateProperty(string type, string name, bool isFromIOC)
        {
            var result = SyntaxFactory.PropertyDeclaration(SyntaxFactory.ParseTypeName(type), name)
                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                .AddAccessorListAccessors(
                                    SyntaxFactory.AccessorDeclaration(SyntaxKind.GetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                                    SyntaxFactory.AccessorDeclaration(SyntaxKind.SetAccessorDeclaration).WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken)));

            if (isFromIOC)
            {
                result = result.AddAttributeLists(SyntaxFactory.AttributeList(SyntaxFactory.SingletonSeparatedList(
                    SyntaxFactory.Attribute(SyntaxFactory.IdentifierName("FromIOC"))
                    )));
            }

            return result;
        }
    }
}
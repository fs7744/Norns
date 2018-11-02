using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.AOP.Codegen
{
    public class ProxyCSharpSyntaxWalker : CSharpSyntaxWalker
    {
        public List<ClassDeclarationSyntax> ClassDeclarations { get; } = new List<ClassDeclarationSyntax>();

        private void VisitMembers(SyntaxList<MemberDeclarationSyntax> members)
        {
            foreach (var item in members)
            {
                item.Accept(this);
            }
        }

        public override void VisitCompilationUnit(CompilationUnitSyntax node)
        {
            VisitMembers(node.Members);
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            VisitMembers(node.Members);
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            if (!node.Identifier.ToString().EndsWith("Attribute")
                && node.Modifiers.Any(i => i.Text == "public") 
                && !node.Modifiers.Any(i => i.Text == "static" || i.Text == "sealed" || i.Text == "abstract" || i.Text == "partial")
                && !node.AttributeLists.SelectMany(i => i.Attributes).Any(i => i.Name.ToString().EndsWith("NoIntercept"))
                && (node.BaseList == null 
                    || !node.BaseList.Types.Select(i => i.Type).Any(i => i.ToString().EndsWith("InterceptorBase")))
                && node.Members.Any(i => 
                {
                    return i is MethodDeclarationSyntax dec
                    && dec.Modifiers.Any(x => x.Text == "public")
                    && dec.Modifiers.Any(x => x.Text == "virtual" || x.Text == "override")
                    && !dec.Modifiers.Any(x => x.Text == "static")
                    && !dec.AttributeLists.SelectMany(x => x.Attributes).Any(x => x.Name.ToString().EndsWith("NoIntercept"));
                }))
            {
                ClassDeclarations.Add(node);
            }
            VisitMembers(node.Members);
        }
    }
}

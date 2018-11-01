using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System.Collections.Generic;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Norns.AOP.Codegen
{
    public class ProxyCodeGenerator
    {
        private readonly AdhocWorkspace workspace;

        public string SrcDirectory { get; set; }
        public string OutputDirectory { get; set; }

        public ProxyCodeGenerator()
        {
            workspace = new AdhocWorkspace();
        }

        public void Generate()
        {
            var files = CSharpFileUtils.ListAllCSharpFile(SrcDirectory);
            var walker = new ProxyCSharpSyntaxWalker();
            foreach (var file in files)
            {
                walker.Visit(CSharpSyntaxTree.ParseText(File.ReadAllText(file)).GetRoot());
            }
            
        }
    }
}

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections.Generic;

namespace Norns.Destiny.JIT.Coder
{
    public class JitOptions
    {
        public CSharpCompilationOptions CompilationOptions { get; set; }

        public CSharpParseOptions ParseOptions { get; set; }

        public IEnumerable<MetadataReference> References { get; set; }
    }
}
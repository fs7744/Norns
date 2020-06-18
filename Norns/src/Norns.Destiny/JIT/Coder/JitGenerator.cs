using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Utils;
using System.Text;

namespace Norns.Destiny.JIT.Coder
{
    public abstract class JitGenerator<T>
    {
        protected abstract JitOptions CreateOptions();

        protected abstract INotationGenerator CreateNotationGenerator();

        protected abstract T Complie(CSharpCompilation compilation);

        public T Generate(ISymbolSource source)
        {
            var notation = CreateNotationGenerator().GenerateNotations(source);
            var sb = new StringBuilder();
            notation.Record(sb);
            var options = CreateOptions();
            var compileTrees = CSharpSyntaxTree.ParseText(sb.ToString(), options.ParseOptions);
            var compilation = CSharpCompilation.Create(RandomUtils.NewName(), new SyntaxTree[] { compileTrees }, options.References, options.CompilationOptions);
            return Complie(compilation);
        }
    }
}
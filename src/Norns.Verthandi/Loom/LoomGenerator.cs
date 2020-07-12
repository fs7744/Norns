using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Norns.Destiny.Loom;
using Norns.Destiny.Notations;
using Norns.Destiny.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Norns.Verthandi.Loom
{
    public abstract class LoomGenerator<T>
    {
        protected abstract LoomOptions CreateOptions();

        protected abstract IEnumerable<INotationGenerator> CreateNotationGenerators();

        protected abstract T Complie(CSharpCompilation compilation);

        public T Generate(ISymbolSource source)
        {
            var sb = new StringBuilder();
            CreateNotationGenerators()
                .Select(i => i.GenerateNotations(source))
                .Combine().Record(sb);
            var code = sb.ToString();
            var options = CreateOptions();
            var compileTrees = CSharpSyntaxTree.ParseText(code, options.ParseOptions);
            var references = AppDomain.CurrentDomain.GetAssemblies()
                .Where(i => !i.IsDynamic && !string.IsNullOrWhiteSpace(i.Location))
                .Distinct()
                .Select(i => MetadataReference.CreateFromFile(i.Location));
            var compilation = CSharpCompilation.Create(RandomUtils.NewName(), new SyntaxTree[] { compileTrees }, references, options.CompilationOptions);
            return Complie(compilation);
        }
    }
}
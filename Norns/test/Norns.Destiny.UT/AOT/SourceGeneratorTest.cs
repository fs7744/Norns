using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Norns.Destiny.Utils;
using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Xunit;

namespace Norns.Destiny.UT.AOT
{
    public static class SourceGeneratorTest
    {
        public static readonly CSharpParseOptions Regular = new CSharpParseOptions(kind: SourceCodeKind.Regular, documentationMode: DocumentationMode.Parse);
        public static readonly CSharpCompilationOptions DebugDll = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release);

        public static string GenerateCode(string source, ISourceGenerator sourceGenerator)
        {
            Compilation compilation = CSharpCompilation.Create(RandomUtils.NewName(),
                new[] { SyntaxFactory.ParseSyntaxTree(SourceText.From(source, Encoding.UTF8), Regular, "") },
                AppDomain.CurrentDomain.GetAssemblies().Where(i => !i.IsDynamic).Select(i => AssemblyMetadata.CreateFromFile(i.Location).GetReference()),
                DebugDll);

            GeneratorDriver driver = new CSharpGeneratorDriver(Regular, ImmutableArray.Create(sourceGenerator), ImmutableArray<AdditionalText>.Empty);
            driver.RunFullGeneration(compilation, out var outputCompilation, out var diagnostics);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            return array[1].ToString();
        }
    }
}
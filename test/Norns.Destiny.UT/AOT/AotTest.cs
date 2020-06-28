using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOT.Coder;
using Norns.Destiny.Notations;
using Norns.Destiny.Utils;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Xunit;

namespace Norns.Destiny.UT.AOT
{
    public static class AotTest
    {
        public static readonly IEnumerable<MetadataReference> References = AppDomain.CurrentDomain.GetAssemblies().Where(i => !i.IsDynamic).Select(i => AssemblyMetadata.CreateFromFile(i.Location).GetReference()).ToArray();
        public static readonly CSharpParseOptions Regular = new CSharpParseOptions(kind: SourceCodeKind.Regular, documentationMode: DocumentationMode.Parse);
        public static readonly CSharpCompilationOptions DebugDll = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release);

        public static string GenerateCode(string code, ISourceGenerator sourceGenerator)
        {
            Compilation compilation = CSharpCompilation.Create(RandomUtils.NewName(),
                new[] { SyntaxFactory.ParseSyntaxTree(SourceText.From(code, Encoding.UTF8), Regular, "") },
                References,
                DebugDll);

            GeneratorDriver driver = new CSharpGeneratorDriver(Regular, ImmutableArray.Create(sourceGenerator), ImmutableArray<AdditionalText>.Empty);
            driver.RunFullGeneration(compilation, out var outputCompilation, out var diagnostics);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            return array[1].ToString();
        }

        public static List<ITypeSymbolInfo> GenerateTypeSymbolInfos(string code, Func<ITypeSymbolInfo, bool> filter)
        {
            var collector = new TypeSymbolInfoCollector(filter);
            GenerateCode(code, collector);
            return collector.Infos;
        }

        public static Dictionary<string, ITypeSymbolInfo> SimpleGenerateTypeSymbolInfos(string code, bool isUseShortName = true)
        {
            var collector = new TypeSymbolInfoCollector(i => i.Namespace == "Norns.Destiny.UT.AOT.Generated");
            GenerateCode($"namespace Norns.Destiny.UT.AOT.Generated {{ {code} }}", collector);
            return collector.Infos.ToDictionary(i => isUseShortName ? i.Name : i.FullName, i => i);
        }
    }

    public class TypeSymbolInfoCollector : AotSourceGeneratorBase
    {
        public List<ITypeSymbolInfo> Infos = new List<ITypeSymbolInfo>();
        private readonly Func<ITypeSymbolInfo, bool> filter;

        public TypeSymbolInfoCollector(Func<ITypeSymbolInfo, bool> filter)
        {
            this.filter = filter;
        }

        public class NothingNotationGenerator : INotationGenerator
        {
            private readonly List<ITypeSymbolInfo> infos;

            public NothingNotationGenerator(List<ITypeSymbolInfo> infos)
            {
                this.infos = infos;
            }

            public INotation GenerateNotations(ISymbolSource source)
            {
                infos.AddRange(source.GetTypes());
                return ConstNotations.Nothing;
            }
        }

        protected override IEnumerable<INotationGenerator> CreateNotationGenerators()
        {
            yield return new NothingNotationGenerator(Infos);
        }

        protected override bool Filter(ITypeSymbolInfo type)
        {
            return filter(type);
        }
    }
}
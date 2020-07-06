using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Microsoft.Extensions.DependencyInjection;
using Norns.Destiny.AOP;
using Norns.Destiny.Loom;
using Norns.Destiny.Notations;
using Norns.Destiny.Structure;
using Norns.Destiny.Utils;
using Norns.Skuld.AOP;
using Norns.Skuld.Loom;
using Norns.Skuld.Structure;
using Norns.Verthandi.Loom;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Xunit;

namespace Norns.Destiny.UT.Skuld
{
    public class EmptyAotAopSourceGenerator : AopSourceGenerator
    {
        protected override IEnumerable<IInterceptorGenerator> GetInterceptorGenerators()
        {
            yield return new Verthandi.AOP.AddSomeTingsInterceptorGenerator();
        }
    }

    public static class SkuldTest
    {
        public static readonly IEnumerable<MetadataReference> References = AppDomain.CurrentDomain.GetAssemblies().Where(i => !i.IsDynamic).Select(i => AssemblyMetadata.CreateFromFile(i.Location).GetReference()).ToArray();
        public static readonly CSharpParseOptions Regular = new CSharpParseOptions(kind: SourceCodeKind.Regular, documentationMode: DocumentationMode.Parse);
        public static readonly CSharpCompilationOptions DebugDll = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Release);

        public static TypeSymbolInfo GetTypeByMetadataName(string fullyQualifiedMetadataName)
        {
            Compilation compilation = CSharpCompilation.Create(RandomUtils.NewName(), null,
               References,
               DebugDll);
            var type = compilation.GetTypeByMetadataName(fullyQualifiedMetadataName);
            return type == null ? null : new TypeSymbolInfo(type);
        }

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

        public static Assembly GenerateAssembly(string code, ISourceGenerator sourceGenerator)
        {
            Compilation compilation = CSharpCompilation.Create(RandomUtils.NewName(),
                  new[] { SyntaxFactory.ParseSyntaxTree(SourceText.From(code, Encoding.UTF8), Regular, "") },
                  References,
                  DebugDll);

            GeneratorDriver driver = new CSharpGeneratorDriver(Regular, ImmutableArray.Create(sourceGenerator), ImmutableArray<AdditionalText>.Empty);
            driver.RunFullGeneration(compilation, out var outputCompilation, out var diagnostics);
            //var array = outputCompilation.SyntaxTrees.ToArray();
            //var gemerateCode = array[1].ToString();
            if (!diagnostics.IsEmpty)
            {
                throw new ComplieException(diagnostics);
            }
            using (var stream = new MemoryStream())
            {
                var restult = outputCompilation.Emit(stream);
                if (restult.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return AssemblyLoadContext.Default.LoadFromStream(stream);
                }
                else
                {
                    throw new ComplieException(restult.Diagnostics);
                }
            }
        }

        public static dynamic GenerateProxy(string code, string typeName, Type[] genericeType = null)
        {
            var assembly = GenerateAssembly($"namespace Norns.Destiny.UT.AOT.Generated {{{code}}}", new EmptyAotAopSourceGenerator());
            var services = new ServiceCollection();
            var type = assembly.GetTypes().First(i => i.Name == typeName);
            if (genericeType != null)
            {
                type = type.MakeGenericType(genericeType);
            }
            services.AddDestinyInterface(type);
            var provider = services.BuildAopServiceProvider(assembly);
            return provider.GetRequiredService(type);
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

    public class TypeSymbolInfoCollector : SourceGeneratorBase
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
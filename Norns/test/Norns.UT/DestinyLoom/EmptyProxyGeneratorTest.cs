using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using Norns.DestinyLoom;
using Norns.DestinyLoom.Test;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Xunit;

namespace Norns.UT.DestinyLoom
{
    public class EmptyProxyGenerator : AbstractProxyGenerator
    {
        public override IEnumerable<IInterceptorGenerator> FindInterceptorGenerators()
        {
            yield return new EmptyInterceptorGenerator();
        }

        public override bool CanProxy(INamedTypeSymbol @type)
        {
            return @type.ToDisplayString().StartsWith("Norns");
        }
    }

    public class EmptyInterceptorGenerator : IInterceptorGenerator
    {
        public IEnumerable<string> AfterMethod(ProxyMethodGeneratorContext context)
        {
            return new string[0];
        }

        public IEnumerable<string> BeforeMethod(ProxyMethodGeneratorContext context)
        {
            return new string[0];
        }
    }

    public class EmptyProxyGeneratorTest
    {
        public static readonly CSharpParseOptions Regular = new CSharpParseOptions(kind: SourceCodeKind.Regular, documentationMode: DocumentationMode.Parse);
        public static readonly CSharpCompilationOptions DebugDll = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary, optimizationLevel: OptimizationLevel.Debug);

        public static string GetUniqueName()
        {
            return Guid.NewGuid().ToString("D");
        }

        private static Compilation GenerateSource(string source)
        {
            Compilation compilation = CSharpCompilation.Create(GetUniqueName(),
                new[] { SyntaxFactory.ParseSyntaxTree(SourceText.From(source, Encoding.UTF8), Regular, "") },
                AppDomain.CurrentDomain.GetAssemblies().Where(i => !i.IsDynamic).Select(i => AssemblyMetadata.CreateFromFile(i.Location).GetReference()),
                DebugDll);

            GeneratorDriver driver = new CSharpGeneratorDriver(Regular, ImmutableArray.Create<ISourceGenerator>(new ProxyGenerator()), ImmutableArray<AdditionalText>.Empty);
            driver.RunFullGeneration(compilation, out var outputCompilation, out var diagnostics);
            return outputCompilation;
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndVoidMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        void AddOne();
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public void AddOne() {  }", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndReturnIntMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        int AddOne(int v);
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public int AddOne(int v)", str);
            Assert.Contains("= default(int)", str);
            Assert.Contains("return r", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceHasDefaultMethodAndReturnIntMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        public int AddOne(int v) 
        {
            return v + 1;
        }
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public int AddOne(int v)", str);
            Assert.Contains("= default(int)", str);
            Assert.Contains("return r", str);
            Assert.Contains("= base.AddOne(v);", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndReturnValueTupleMethod()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        (int,int) AddOne(int v);
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public (int, int) AddOne(int v)", str);
            Assert.Contains("= default((int, int))", str);
            Assert.Contains("return r", str);
        }

        [Fact]
        public void GenerateProxyClassWhenInterfaceAndReturnValueListIntMethod()
        {
            var source = @"
using System.Collections.Generic;
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        List<int> AddOne(int v);
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public System.Collections.Generic.List<int> AddOne(int v)", str);
            Assert.Contains("= default(System.Collections.Generic.List<int>)", str);
            Assert.Contains("return r", str);
        }

        [Fact]
        public void GenerateProxyClass()
        {
            var source = @"
namespace Norns.ProxyGenerators.Test
{
    public interface IC
    {
        int AddOne(int v);
    }

    public class C : IC
    {
        public int AddOne(int v)
{
    return v + 1;
}
    }
}
";
            Compilation outputCompilation = GenerateSource(source);
            var array = outputCompilation.SyntaxTrees.ToArray();
            Assert.Equal(2, array.Length);
            var str = array[1].ToString();
            Assert.Contains("ProxyIC", str);
            Assert.Contains(": Norns.ProxyGenerators.Test.IC", str);
            Assert.Contains("public int AddOne(int v)", str);
        }
    }
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Loader;

namespace Norns.CompilerTestds { public class A : Norns.CompilerTest.IC { public int V { get; } = 5; public string P()
        {
            return V.ToString();
        }
    } }

namespace Norns.CompilerTest
{
    public interface IC
    {
        string P();
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            Test();
            GC.Collect();
            GC.Collect();
            GC.Collect();
            Console.ReadKey();
        }

        private static void Test2()
        {
            var b = Activator.CreateInstance(typeof(Norns.CompilerTestds.A)) as IC;
            Console.WriteLine(b.GetType().FullName);
            Console.WriteLine(b?.P());
        }

        private static void Test()
        {
            var script = "namespace Norns.CompilerTestdsd { public class A : Norns.CompilerTest.IC { public int V {get;} = 5; public string P() { return V.ToString();} } }";
            var references = AppDomain.CurrentDomain.GetAssemblies().Select(i => MetadataReference.CreateFromFile(i.Location)).ToArray();
            var compilationOptions = new CSharpCompilationOptions(
                                   concurrentBuild: true,
                                   metadataImportOptions: MetadataImportOptions.All,
                                   outputKind: OutputKind.DynamicallyLinkedLibrary,
                                   optimizationLevel: OptimizationLevel.Release,
                                   allowUnsafe: true,
                                   platform: Platform.AnyCpu,
                                   checkOverflow: false,
                                   assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);
            var compileTrees = CSharpSyntaxTree.ParseText(script.Trim(), new CSharpParseOptions(LanguageVersion.Latest, preprocessorSymbols: new[] { "RELEASE" }));
            var compilation = CSharpCompilation.Create("adsd", new SyntaxTree[] { compileTrees }, references, compilationOptions);
            MemoryStream stream = new MemoryStream();
            var restult = compilation.Emit(stream);
            if (restult.Success)
            {
                stream.Seek(0, SeekOrigin.Begin);
                var a = AssemblyLoadContext.Default.LoadFromStream(stream);
                stream.Dispose();
                var b = a.CreateInstance("Norns.CompilerTestdsd.A") as IC;
                Console.WriteLine(b.GetType().FullName);
                Console.WriteLine(b?.P());
            }
            else
            {
                foreach (var item in restult.Diagnostics)
                {
                    Console.WriteLine(item.ToString());
                }
                Console.WriteLine(restult.Success);
            }
        }
    }
}
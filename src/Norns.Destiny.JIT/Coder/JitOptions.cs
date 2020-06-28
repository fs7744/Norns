using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOP;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.JIT.Coder
{
    public class JitOptions
    {
        public Func<ITypeSymbolInfo, bool> FilterForDefaultImplement { get; set; }

        public CSharpCompilationOptions CompilationOptions { get; set; }

        public CSharpParseOptions ParseOptions { get; set; }

        public IEnumerable<MetadataReference> References { get; set; }

        public static JitOptions CreateDefault()
        {
            var parseOptions = new CSharpParseOptions(LanguageVersion.Latest, preprocessorSymbols: new[] { "RELEASE" });
            var compilationOptions = new CSharpCompilationOptions(
                                   concurrentBuild: true,
                                   metadataImportOptions: MetadataImportOptions.All,
                                   outputKind: OutputKind.DynamicallyLinkedLibrary,
                                   optimizationLevel: OptimizationLevel.Release,
                                   allowUnsafe: true,
                                   platform: Platform.AnyCpu,
                                   checkOverflow: false,
                                   assemblyIdentityComparer: DesktopAssemblyIdentityComparer.Default);
            var references = AppDomain.CurrentDomain.GetAssemblies().Where(i => !i.IsDynamic).Select(i => MetadataReference.CreateFromFile(i.Location)).ToArray();
            return new JitOptions()
            {
                CompilationOptions = compilationOptions,
                FilterForDefaultImplement = AopUtils.CanDoDefaultImplement,
                ParseOptions = parseOptions,
                References = references
            };
        }
    }
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Norns.Destiny.Structure;
using System;

namespace Norns.Verthandi.Loom
{
    public class LoomOptions
    {
        public Func<ITypeSymbolInfo, bool> FilterProxy { get; set; }

        public Func<ITypeSymbolInfo, bool> FilterForDefaultImplement { get; set; }

        public CSharpCompilationOptions CompilationOptions { get; set; }

        public CSharpParseOptions ParseOptions { get; set; }

        public static LoomOptions CreateDefault()
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
            return new LoomOptions()
            {
                CompilationOptions = compilationOptions,
                FilterProxy = i => !i.Namespace.StartsWith("System") && !i.Namespace.StartsWith("Microsoft"),
                FilterForDefaultImplement = i => !i.Namespace.StartsWith("System") && !i.Namespace.StartsWith("Microsoft"),
                ParseOptions = parseOptions
            };
        }
    }
}
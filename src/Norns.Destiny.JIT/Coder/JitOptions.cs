using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.AOP;
using Norns.Destiny.Attributes;
using System;

namespace Norns.Destiny.JIT.Coder
{
    public class JitOptions
    {
        public Func<ITypeSymbolInfo, bool> FilterProxy { get; set; }

        public Func<ITypeSymbolInfo, bool> FilterForDefaultImplement { get; set; }

        public CSharpCompilationOptions CompilationOptions { get; set; }

        public CSharpParseOptions ParseOptions { get; set; }

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
            return new JitOptions()
            {
                CompilationOptions = compilationOptions,
                FilterProxy = i => AopUtils.CanAopType(i) && i.HasAttribute<CharonAttribute>(),
                FilterForDefaultImplement = AopUtils.CanDoDefaultImplement,
                ParseOptions = parseOptions
            };
        }
    }
}
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Norns.Destiny.AOP;
using Norns.Destiny.Attributes;
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
                FilterProxy = i => i.Accessibility == AccessibilityInfo.Public && i.Namespace != null &&  !i.Namespace.StartsWith("System") && !i.Namespace.StartsWith("Internal.") && AopUtils.CanAopType(i) && i.HasAttribute<CharonAttribute>() ,
                FilterForDefaultImplement = i => i.Accessibility == AccessibilityInfo.Public && i.Namespace != null && !i.Namespace.StartsWith("System") && !i.Namespace.StartsWith("Internal.") && AopUtils.CanDoDefaultImplement(i) && i.HasAttribute<CharonAttribute>(),
                ParseOptions = parseOptions
            };
        }
    }
}
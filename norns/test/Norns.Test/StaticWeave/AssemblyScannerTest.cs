using Mono.Cecil;
using Norns.AOP.Attributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xunit;
using Norns.StaticWeave;

namespace Norns.Test.StaticWeave
{
    public class AssemblyScannerTest
    {
        [Fact]
        public void FindNeedProxyClass()
        {
            var dllPath = Path.Combine(Directory.GetCurrentDirectory(), "debugdll", "TestFuncToDll.dll");
            var assembly = AssemblyDefinition.ReadAssembly(dllPath, new ReaderParameters() { ReadSymbols = true, ReadWrite = true });
           var types = assembly.Modules.SelectMany(i => i.Types).Where(i => 
           {
               return i.CustomAttributes.Count > 0 && i.CustomAttributes[0].AttributeType.BaseTypeIs(typeof(InterceptorBaseAttribute));
           }).ToArray();
        }
    }
}

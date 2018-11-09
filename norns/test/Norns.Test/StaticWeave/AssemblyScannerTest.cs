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
           var assembly = AssemblyDefinition.ReadAssembly(dllPath, new ReaderParameters() { ReadSymbols = false });
           var types = assembly.GetNeedInterceptTypes().ToArray();
           var methods = types[0].GetNeedInterceptMethods();
        }
    }
}

using Norns.AOP.Codegen;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace Norns.Test.Codegen
{
    public class ProxyCodeGeneratorTest
    {
        [Fact]
        public void Test()
        {
            var projectDirectory = Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.ToString();
            new ProxyCodeGenerator()
            {
                SrcDirectory = projectDirectory,
                OutputDirectory = Path.Combine(projectDirectory, "obj"),
            }.Generate();
        }
    }
}

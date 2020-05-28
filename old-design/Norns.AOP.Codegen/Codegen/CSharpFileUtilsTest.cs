using Norns.AOP.Codegen;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Norns.Test.Codegen
{
    public class CSharpFileUtilsTest
    {
        [Fact]
        public void ListAllCSharpFile()
        {
            var files = CSharpFileUtils.ListAllCSharpFile(Directory.GetParent(Directory.GetCurrentDirectory()).Parent.Parent.ToString()).ToArray();
            Assert.NotEmpty(files);
            foreach (var file in files)
            {
                file.EndsWith(".cs", StringComparison.OrdinalIgnoreCase);
            }
        }
    }
}
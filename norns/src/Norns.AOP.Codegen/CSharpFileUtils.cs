using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Norns.AOP.Codegen
{
    public static class CSharpFileUtils
    {
        private const string CSharpFilePattern = "*.cs";

        public static IEnumerable<string> ListAllCSharpFile(string src)
        {
            return Directory.EnumerateFiles(src, CSharpFilePattern, SearchOption.TopDirectoryOnly)
                .Union(Directory.EnumerateDirectories(src, "*", SearchOption.TopDirectoryOnly)
                .Where(i => !i.EndsWith("bin", StringComparison.OrdinalIgnoreCase)
                    && !i.EndsWith("obj", StringComparison.OrdinalIgnoreCase))
                    .SelectMany(i => Directory.EnumerateFiles(i, CSharpFilePattern)));
        }
    }
}
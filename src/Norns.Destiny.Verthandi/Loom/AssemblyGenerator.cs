using Microsoft.CodeAnalysis.CSharp;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Norns.Verthandi.Loom
{
    public abstract class AssemblyGenerator : LoomGenerator<Assembly>
    {
        protected override Assembly Complie(CSharpCompilation compilation)
        {
            using (var stream = new MemoryStream())
            {
                var restult = compilation.Emit(stream);
                if (restult.Success)
                {
                    stream.Seek(0, SeekOrigin.Begin);
                    return AssemblyLoadContext.Default.LoadFromStream(stream);
                }
                else
                {
                    throw new ComplieException(restult.Diagnostics);
                }
            }
        }
    }
}
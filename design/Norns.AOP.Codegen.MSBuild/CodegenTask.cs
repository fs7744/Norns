using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System.IO;

namespace Norns.AOP.Codegen.MSBuild
{
    public class CodegenTask : Task
    {
        [Required]
        public string MSBuildProjectDirectory { get; set; }

        [Required]
        public string Configuration { get; set; }

        public override bool Execute()
        {
            try
            {
                new ProxyCodeGenerator()
                {
                    SrcDirectory = MSBuildProjectDirectory,
                    OutputDirectory = Path.Combine(MSBuildProjectDirectory, "obj"),
                }
                .Generate();
                return true;
            }
            catch (System.Exception ex)
            {
                Log.LogError(ex.Message);
                return false;
            }
        }
    }
}
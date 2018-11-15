using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;

namespace Norns.StaticWeave.MSBuild
{
    public class WeavingTask : Task
    {
        [Required]
        public string MSBuildProjectDirectory { get; set; }

        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, $"Hello {MSBuildProjectDirectory}");
            return true;
        }
    }
}

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using System;

namespace TestMSBuild
{
    public class SayHelloTask : Task
    {
        public override bool Execute()
        {
            Log.LogMessage(MessageImportance.High, "Hello MSBuild");
            return true;
        }
    }
}

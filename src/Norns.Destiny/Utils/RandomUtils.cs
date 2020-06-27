using System;

namespace Norns.Destiny.Utils
{
    public static class RandomUtils
    {
        public static string NewName()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static string NewCSFileName()
        {
            return $"{NewName()}.cs";
        }
    }
}
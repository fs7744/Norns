using System;

namespace Norns.DestinyLoom
{
    public static class GuidHelper
    {
        public static string NewGuidName()
        {
            return Guid.NewGuid().ToString("N");
        }
    }
}
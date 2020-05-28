using System;
using System.Collections.Generic;
using System.Text;

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

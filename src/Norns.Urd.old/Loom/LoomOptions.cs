using Norns.Destiny.Structure;
using System;

namespace Norns.Urd.Loom
{
    public class LoomOptions
    {
        public Func<ITypeSymbolInfo, bool> FilterProxy { get; set; }

        public Func<ITypeSymbolInfo, bool> FilterForDefaultImplement { get; set; }

        public static LoomOptions CreateDefault()
        {
            return new LoomOptions()
            {
                FilterProxy = i => !i.Namespace.StartsWith("System") && !i.Namespace.StartsWith("Microsoft"),
                FilterForDefaultImplement = i => !i.Namespace.StartsWith("System") && !i.Namespace.StartsWith("Microsoft"),
            };
        }
    }
}
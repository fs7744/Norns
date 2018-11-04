using System;
using System.Collections.Generic;
using System.Text;

namespace Norns.DependencyInjection
{
    internal abstract class ServiceCallSite
    {
        public abstract Type ServiceType { get; }
        public abstract Type ImplementationType { get; }
    }
}

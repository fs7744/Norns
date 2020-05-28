using System;

namespace Norns.DependencyInjection
{
    public abstract class ServiceDefintion
    {
        public abstract Type ServiceType { get; }
        public abstract Type ImplementationType { get; }
        public abstract Lifetime Lifetime { get; }
        public abstract string Name { get; }
    }
}
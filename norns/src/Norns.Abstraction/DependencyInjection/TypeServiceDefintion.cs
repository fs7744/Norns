using System;

namespace Norns.DependencyInjection
{
    public class TypeServiceDefintion : ServiceDefintion
    {
        public override Type ServiceType { get; }
        public override Type ImplementationType { get; }
        public override Lifetime Lifetime { get; }
        public override string Name { get; }

        public TypeServiceDefintion(Type serviceType, Type implementationType, Lifetime lifetime, string name)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            Name = name;
        }
    }
}
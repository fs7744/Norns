using System;

namespace Norns.DependencyInjection
{
    public class DelegateServiceDefintion : ServiceDefintion, IImplementationFactory
    {
        public DelegateServiceDefintion(Type serviceType, Type implementationType, Lifetime lifetime,
            Func<INamedServiceProvider, object> implementationFactory, string name)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            ImplementationFactory = implementationFactory;
            Name = name;
        }

        public override Type ServiceType { get; }

        public override Type ImplementationType { get; }

        public override Lifetime Lifetime { get; }

        public override string Name { get; }

        public Func<INamedServiceProvider, object> ImplementationFactory { get; }
    }
}
using System;

namespace Norns.DependencyInjection
{
    public class DelegateServiceDefintion : ServiceDefintion, IImplementationFactory
    {
        public DelegateServiceDefintion(Type serviceType, Type implementationType, Lifetime lifetime,
            Func<IServiceProvider, object> implementationFactory)
        {
            ServiceType = serviceType;
            ImplementationType = implementationType;
            Lifetime = lifetime;
            ImplementationFactory = implementationFactory;
        }

        public override Type ServiceType { get; }

        public override Type ImplementationType { get; }

        public override Lifetime Lifetime { get; }

        public Func<IServiceProvider, object> ImplementationFactory { get; }
    }
}
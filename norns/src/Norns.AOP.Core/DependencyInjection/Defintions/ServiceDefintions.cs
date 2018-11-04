using System;
using System.Collections;
using System.Collections.Generic;

namespace Norns.DependencyInjection
{
    public class ServiceDefintions : IServiceDefintions
    {
        private readonly List<ServiceDefintion> services = new List<ServiceDefintion>();

        public void Add(ServiceDefintion serviceDefintion)
        {
            if (serviceDefintion == null)
            {
                throw new ArgumentNullException(nameof(serviceDefintion));
            }
            services.Add(serviceDefintion);
        }

        public static ServiceDefintion Define(Type serviceType, Type implementationType, Lifetime lifetime,
            Func<IServiceProvider, object> objectFactory = null)
        {
            if (serviceType == null)
            {
                throw new ArgumentNullException(nameof(serviceType));
            }
            if (implementationType == null)
            {
                throw new ArgumentNullException(nameof(implementationType));
            }
            if (!serviceType.IsAssignableFrom(implementationType))
            {
                throw new ArgumentException($"{implementationType} is not assignable from {serviceType}.");
            }
            if (objectFactory != null)
            {
                return new DelegateServiceDefintion(serviceType, implementationType, lifetime, objectFactory);
            }
            //else if (serviceType.IsConstructedGenericType)
            //{
            //    switch (serviceType.GetGenericTypeDefinition())
            //    {
            //        case Type enumerable when enumerable == typeof(IEnumerable<>):
            //            return FindEnumerable(serviceType);
            //        case Type genericTypeDefinition:
            //            return FindGenericService(serviceType, genericServiceDefinitions);
            //        default:
            //            throw new NotSupportedException($"ServiceDefintion not supported serviceType: {serviceType}.");
            //    }
            //}
            else
            {
                return new TypeServiceDefintion(serviceType, implementationType, lifetime);
            }
        }

        public IEnumerator<ServiceDefintion> GetEnumerator()
        {
            return services.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
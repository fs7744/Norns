using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Text;

namespace Norns.DependencyInjection
{
    public class ServiceDefintionFactory
    {
        private readonly ConcurrentDictionary<Type, LinkedList<DelegateServiceDefintion>> cache;
        //private readonly ConcurrentDictionary<Type, LinkedList<DelegateServiceDefintion>> genericCache;

        public ServiceDefintionFactory(IEnumerable<ServiceDefintion> services)
        {
            cache = new ConcurrentDictionary<Type, LinkedList<DelegateServiceDefintion>>();
            //genericCache = new ConcurrentDictionary<Type, LinkedList<DelegateServiceDefintion>>();
            Fill(services);
        }

        private void Fill(IEnumerable<ServiceDefintion> services)
        {
            foreach (var service in services)
            {
                if (service is DelegateServiceDefintion delegateDefintion)
                {
                    var list = cache.GetOrAdd(service.ServiceType, i => new LinkedList<DelegateServiceDefintion>());
                    list.Add(delegateDefintion);
                }
            }
        }

        public DelegateServiceDefintion TryGet(Type serviceType)
        {
            DelegateServiceDefintion defintion = null;
            if (cache.TryGetValue(serviceType, out LinkedList<DelegateServiceDefintion> list))
            {
                defintion = list.Last.Value;
            }
            return defintion;
        }
    }
}

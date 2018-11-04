//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using Microsoft.Extensions.DependencyInjection;

//namespace Norns.DependencyInjection
//{
//    internal class CallSiteFactory
//    {
//        private readonly ConcurrentDictionary<Type, LinkedList<ServiceCallSite>> cache;
//        private readonly ConcurrentDictionary<Type, LinkedList<ServiceCallSite>> genericCache;

//        public CallSiteFactory(IEnumerable<ServiceDescriptor> serviceDescriptors)
//        {
//            cache = new ConcurrentDictionary<Type, LinkedList<ServiceCallSite>>();
//            genericCache = new ConcurrentDictionary<Type, LinkedList<ServiceCallSite>>();
//            Fill(serviceDescriptors);
//        }

//        private void Fill(IEnumerable<ServiceDescriptor> serviceDescriptors)
//        {
//            foreach (var descriptor in serviceDescriptors)
//            {
//                if (descriptor.ServiceType.ContainsGenericParameters)
//                {
//                    var callSites = genericCache.GetOrAdd(descriptor.ServiceType.GetGenericTypeDefinition(), i => new LinkedList<ServiceCallSite>());
//                    //callSites.Add(descriptor);
//                }
//                else
//                {
//                    var callSites = cache.GetOrAdd(descriptor.ServiceType, i => new LinkedList<ServiceCallSite>());
//                    callSites.Add(CreateCallSite(descriptor));
//                }
//            }
//        }

//        private ServiceCallSite CreateCallSite(ServiceDescriptor descriptor)
//        {
//            throw new NotImplementedException();
//        }

//        internal static ServiceCallSite GetCallSite(Type serviceType, CallSiteChain callSiteChain)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
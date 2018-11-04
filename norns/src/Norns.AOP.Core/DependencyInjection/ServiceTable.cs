//using System;
//using System.Collections.Concurrent;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using Microsoft.Extensions.DependencyInjection;

//namespace Norns.DependencyInjection
//{
//    internal class ServiceTable
//    {
//        private readonly ConcurrentDictionary<Type, LinkedList<ServiceDescriptor>> linkedServiceDescriptors;
//        private readonly ConcurrentDictionary<Type, LinkedList<ServiceDescriptor>> linkedGenericServiceDescriptors;
//        private readonly bool dynamicInterceptor;

//        public ServiceTable(bool dynamicInterceptor)
//        {
//            linkedServiceDescriptors = new ConcurrentDictionary<Type, LinkedList<ServiceDescriptor>>();
//            linkedGenericServiceDescriptors = new ConcurrentDictionary<Type, LinkedList<ServiceDescriptor>>();
//            this.dynamicInterceptor = dynamicInterceptor;
//        }

//        internal void Fill(IServiceCollection serviceDescriptors)
//        {
//            foreach (var descriptor in serviceDescriptors)
//            {
//                if (descriptor.ServiceType.GetTypeInfo().ContainsGenericParameters)
//                {
//                    var linkedGenericServices = linkedGenericServiceDescriptors.GetOrAdd(descriptor.ServiceType.GetGenericTypeDefinition(), i => new LinkedList<ServiceDescriptor>());
//                    linkedGenericServices.Add(descriptor);
//                }
//                else
//                {
//                    var linkedServices = linkedServiceDescriptors.GetOrAdd(descriptor.ServiceType, i => new LinkedList<ServiceDescriptor>());
//                    linkedServices.Add(MakeProxyDescriptor(descriptor));
//                }
//            }
//        }

//        private ServiceDescriptor MakeProxyDescriptor(ServiceDescriptor descriptor)
//        {
//            return dynamicInterceptor ? null : descriptor;
//        }

//        internal ServiceDescriptor GetServiceDescriptor(Type serviceType)
//        {
//            if (serviceType == null)
//            {
//                return null;
//            }
//            else if (linkedServiceDescriptors.TryGetValue(serviceType, out var value))
//            {
//                return value.Last.Value;
//            }
//            else if (serviceType.IsConstructedGenericType)
//            {
//                switch (serviceType.GetGenericTypeDefinition())
//                {
//                    case Type enumerable when enumerable == typeof(IEnumerable<>):
//                        return FindEnumerableDescriptor(serviceType);
//                    //case Type genericTypeDefinition when linkedGenericServiceDescriptors.TryGetValue(genericTypeDefinition, out var genericServiceDefinitions):
//                    //    return FindGenericDescriptor(serviceType, genericServiceDefinitions);
//                    default:
//                        break;
//                }
//            }
//            return null;
//        }

//        private ServiceDescriptor FindEnumerableDescriptor(Type serviceType)
//        {
//            if (linkedServiceDescriptors.TryGetValue(serviceType, out var value))
//            {
//                return value.Last.Value;
//            }
//            var elementType = serviceType.GetTypeInfo().GetGenericArguments()[0];
//            var elements = FindEnumerableElements(serviceType);
//            linkedServiceDescriptors[serviceType] = new LinkedList<ServiceDescriptor>(elements);
//            return null;
            
//            //return enumerableServiceDefinition;
//        }

//        private ServiceDescriptor[] FindEnumerableElements(Type serviceType)
//        {
//            var elementType = serviceType.GetTypeInfo().GetGenericArguments()[0];
//            var services = new List<ServiceDescriptor>();
//            if (linkedServiceDescriptors.TryGetValue(elementType, out var linkedServices))
//            {
//                services.AddRange(linkedServices);
//            }
//            if (elementType.IsConstructedGenericType &&
//                linkedGenericServiceDescriptors.TryGetValue(elementType.GetGenericTypeDefinition(), out var linkedGenericServices))
//            {
//                services.AddRange(linkedGenericServices.Select(x => MakeProxyDescriptor(MakeGenericServiceDescriptor(elementType, x))).Where(x => x != null));
//            }
//            return services.ToArray();
//        }

//        private ServiceDescriptor MakeGenericServiceDescriptor(Type elementType, ServiceDescriptor x)
//        {
//            return null;
//        }
//    }
//}
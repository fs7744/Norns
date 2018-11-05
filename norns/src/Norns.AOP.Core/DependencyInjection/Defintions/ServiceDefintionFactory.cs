using Norns.Extensions.Reflection;
using Norns.Extensions.Reflection.Extensions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Norns.DependencyInjection
{
    public class ServiceDefintionFactory
    {
        private static readonly MethodInfo toArray = typeof(ServiceDefintionFactory).GetMethod("ToArray", BindingFlags.Static | BindingFlags.NonPublic);
        private readonly ConcurrentDictionary<Type, LinkedList<DelegateServiceDefintion>> cache;
        private readonly ConcurrentDictionary<Type, LinkedList<TypeServiceDefintion>> genericCache;

        public ServiceDefintionFactory(IEnumerable<ServiceDefintion> services)
        {
            cache = new ConcurrentDictionary<Type, LinkedList<DelegateServiceDefintion>>();
            genericCache = new ConcurrentDictionary<Type, LinkedList<TypeServiceDefintion>>();
            Fill(services);
        }

        private static object ToArray<T>(IEnumerable<object> list)
        {
            return list.Select(i => (T)i).ToArray();
        }

        private void Fill(IEnumerable<ServiceDefintion> services)
        {
            foreach (var service in services)
            {
                switch (service)
                {
                    case DelegateServiceDefintion delegateDefintion:
                        {
                            var list = cache.GetOrAdd(service.ServiceType, i => new LinkedList<DelegateServiceDefintion>());
                            list.Add(delegateDefintion);
                        }
                        break;

                    case TypeServiceDefintion typeServiceDefintion:
                        {
                            var serviceType = typeServiceDefintion.ServiceType;
                            if (serviceType.ContainsGenericParameters)
                            {
                                var list = genericCache.GetOrAdd(service.ServiceType, i => new LinkedList<TypeServiceDefintion>());
                                list.Add(typeServiceDefintion);
                            }
                            else
                            {
                                TypeToDelegateServiceDefintion(typeServiceDefintion);
                            }
                        }
                        break;

                    default:
                        break;
                }
            }
        }

        private LinkedList<DelegateServiceDefintion> TypeToDelegateServiceDefintion(TypeServiceDefintion typeServiceDefintion)
        {
            var serviceType = typeServiceDefintion.ServiceType;
            var implementationType = typeServiceDefintion.ImplementationType;
            var constructor = implementationType.GetConstructors().FirstOrDefault(i => i.IsPublic);
            if (constructor == null)
            {
                throw new NotSupportedException($"DependencyInjection not supported no public constructor of {implementationType}.");
            }
            var reflector = constructor.GetReflector();
            Func<IServiceProvider, object> func = null;
            var parameters = reflector.Parameters;
            var ctor = reflector.Invoke;
            if (parameters.Length > 0)
            {
                func = i =>
                {
                    var args = new object[parameters.Length];
                    for (int j = 0; j < parameters.Length; j++)
                    {
                        args[j] = i.GetService(parameters[j].Member.ParameterType);
                    }
                    return ctor(args);
                };
            }
            else
            {
                var args = new object[0];
                func = i => ctor(args);
            }
            var defintion = new DelegateServiceDefintion(serviceType, implementationType, typeServiceDefintion.Lifetime, func);
            var list = cache.GetOrAdd(serviceType, i => new LinkedList<DelegateServiceDefintion>());
            list.Add(defintion);
            return list;
        }

        public DelegateServiceDefintion TryGet(Type serviceType)
        {
            var list = TryGetList(serviceType);
            return list == null ? null : list.Last.Value;
        }

        public LinkedList<DelegateServiceDefintion> TryGetList(Type serviceType)
        {
            LinkedList<DelegateServiceDefintion> defintions = null;
            if (cache.TryGetValue(serviceType, out var list))
            {
                defintions = list;
            }
            else if (serviceType.IsConstructedGenericType)
            {
                switch (serviceType.GetGenericTypeDefinition())
                {
                    case Type enumerable when enumerable == typeof(IEnumerable<>):
                        var elementType = serviceType.GetGenericArguments()[0];
                        var elementList = TryGetList(elementType);
                        if (elementList != null)
                        {
                            var method = toArray.MakeGenericMethod(new Type[] { elementType });
                            var dynamicMethod = new DynamicMethod($"invoker-{Guid.NewGuid()}", serviceType, new Type[] { typeof(IEnumerable<object>) }, method.Module, true);
                            var ilGen = dynamicMethod.GetILGenerator();
                            ilGen.EmitLoadArg(0);
                            ilGen.EmitCall(OpCodes.Call, method, null);
                            ilGen.Emit(OpCodes.Ret);
                            var func = (Func<IEnumerable<object>, object>)dynamicMethod.CreateDelegate(typeof(Func<IEnumerable<object>, object>));
                            var lifetime = genericCache.TryGetValue(enumerable, out var genericServiceDefinitions) ? genericServiceDefinitions.Last.Value.Lifetime : Lifetime.Transient;
                            var defintion = new DelegateServiceDefintion(serviceType, serviceType, lifetime, i =>
                            {
                                return func(elementList.Select(j => j.ImplementationFactory(i)).AsEnumerable());
                            });
                            defintions = new LinkedList<DelegateServiceDefintion>() { defintion };
                            cache[serviceType] = defintions;
                        }
                        break;

                    case Type genericTypeDefinition when genericCache.TryGetValue(genericTypeDefinition, out var genericServiceDefinitions):
                        var genericDefinition = genericServiceDefinitions.Last.Value;
                        defintions = TypeToDelegateServiceDefintion(new TypeServiceDefintion(serviceType,
                            genericDefinition.ImplementationType.MakeGenericType(serviceType.GenericTypeArguments), genericDefinition.Lifetime));
                        break;

                    default:
                        break;
                }
            }
            return defintions;
        }
    }
}
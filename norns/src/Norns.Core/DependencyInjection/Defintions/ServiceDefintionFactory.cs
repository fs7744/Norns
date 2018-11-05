using Norns.AOP.Attributes;
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
    [NoIntercept]
    internal class ServiceDefintionFactory : IServiceDefintionFactory
    {
        private static readonly MethodInfo toArray = typeof(ServiceDefintionFactory).GetMethod("ToArray", BindingFlags.Static | BindingFlags.Public);
        private readonly ConcurrentDictionary<Type, LinkedList<DelegateServiceDefintion>> cache;
        private readonly ConcurrentDictionary<Type, LinkedList<TypeServiceDefintion>> genericCache;
        private readonly IDelegateServiceDefintionHandler defintionHandler;

        public ServiceDefintionFactory(IEnumerable<ServiceDefintion> services, IDelegateServiceDefintionHandler defintionHandler)
        {
            this.defintionHandler = defintionHandler;
            cache = new ConcurrentDictionary<Type, LinkedList<DelegateServiceDefintion>>();
            genericCache = new ConcurrentDictionary<Type, LinkedList<TypeServiceDefintion>>();
            Fill(services);
        }

        public static object ToArray<T>(IEnumerable<object> list)
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
                            CacheDelegate(delegateDefintion);
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
            Func<INamedServiceProvider, object> func = null;
            var parameters = reflector.Parameters
                .Select(i => Tuple.Create(i.Member.ParameterType, i.GetCustomAttribute<FromDIAttribute>()?.Named))
                .ToArray();
            var ctor = reflector.Invoke;
            if (parameters.Length > 0)
            {
                func = i =>
                {
                    var args = new object[parameters.Length];
                    for (int j = 0; j < parameters.Length; j++)
                    {
                        var p = parameters[j];
                        args[j] = i.GetService(p.Item1, p.Item2);
                    }
                    return ctor(args);
                };
            }
            else
            {
                var args = new object[0];
                func = i => ctor(args);
            }
            var defintion = new DelegateServiceDefintion(serviceType, implementationType, typeServiceDefintion.Lifetime, func, typeServiceDefintion.Name);
            return CacheDelegate(defintion);
        }

        private LinkedList<DelegateServiceDefintion> CacheDelegate(DelegateServiceDefintion defintion)
        {
            var list = cache.GetOrAdd(defintion.ServiceType, i => new LinkedList<DelegateServiceDefintion>());
            list.Add(defintionHandler.Handle(defintion));
            return list;
        }

        public DelegateServiceDefintion TryGet(Type serviceType, string name)
        {
            var list = TryGetList(serviceType);
            return list == null ? null
                : (name == null ? list.Last.Value : list.LastOrDefault(i => i.Name == name));
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
                            var elementLists = elementList.SkipWhile(i => i.Name == null)
                                .GroupBy(i => i.Name)
                                .Select(i => Tuple.Create(i.Key, i.ToArray()))
                                .Union(new Tuple<string, DelegateServiceDefintion[]>[]
                                {
                                    new Tuple<string, DelegateServiceDefintion[]>(null, elementList.ToArray())
                                })
                                .Select(x => new DelegateServiceDefintion(serviceType, serviceType, lifetime, i =>
                                {
                                    return func(x.Item2.Select(j => j.ImplementationFactory(i)).AsEnumerable());
                                }, x.Item1));
                            defintions = new LinkedList<DelegateServiceDefintion>(elementLists);
                            cache[serviceType] = defintions;
                        }
                        break;

                    case Type genericTypeDefinition when genericCache.TryGetValue(genericTypeDefinition, out var genericServiceDefinitions):
                        defintions = genericServiceDefinitions
                            .GroupBy(i => i.Name)
                            .Select(i =>
                            {
                                var genericDefinition = i.Last();
                                return TypeToDelegateServiceDefintion(new TypeServiceDefintion(serviceType,
                                genericDefinition.ImplementationType.MakeGenericType(serviceType.GenericTypeArguments),
                                genericDefinition.Lifetime, genericDefinition.Name));
                            }).Last();
                        break;

                    default:
                        break;
                }
            }
            return defintions;
        }
    }
}
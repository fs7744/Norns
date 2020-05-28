using Norns.Extensions.Reflection;
using Norns.Extensions.Reflection.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Norns.DependencyInjection
{
    public class PropertyInjector : IDelegateServiceDefintionHandler
    {
        private static readonly MethodInfo getService = typeof(INamedServiceProvider).GetMethod("GetService", new Type[] { typeof(Type), typeof(string) });

        public int Order => 0;

        public DelegateServiceDefintion Handle(DelegateServiceDefintion defintion)
        {
            var properties = defintion.ImplementationType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(i => i.CanWrite && i.GetReflector().IsDefined<FromDIAttribute>())
                .ToArray();

            if (properties.Length == 0)
            {
                return defintion;
            }
            else
            {
                return new DelegateServiceDefintion(defintion.ServiceType, defintion.ImplementationType,
                    defintion.Lifetime, CreatePropertyInjector(properties, defintion.ImplementationFactory), defintion.Name);
            }
        }

        private Func<INamedServiceProvider, object> CreatePropertyInjector(PropertyInfo[] properties
            , Func<INamedServiceProvider, object> implementationFactory)
        {
            var dynamicMethod = new DynamicMethod($"invoker-{Guid.NewGuid()}", typeof(void), new Type[] { typeof(INamedServiceProvider), typeof(object) }, GetType().Module, true);
            var ilGen = dynamicMethod.GetILGenerator();
            foreach (var property in properties)
            {
                ilGen.EmitLoadArg(1);
                ilGen.EmitLoadArg(0);
                ilGen.EmitConstant(property.PropertyType, typeof(Type));
                ilGen.EmitConstant(property.GetReflector().GetCustomAttribute<FromDIAttribute>().Named, typeof(string));
                ilGen.Emit(OpCodes.Callvirt, getService);
                ilGen.EmitConvertFromObject(property.PropertyType);
                ilGen.Emit(OpCodes.Callvirt, property.SetMethod);
            }
            ilGen.Emit(OpCodes.Ret);
            var setter = (Action<INamedServiceProvider, object>)dynamicMethod.CreateDelegate(typeof(Action<INamedServiceProvider, object>));
            return i =>
            {
                var result = implementationFactory(i);
                if (result != null)
                {
                    setter(i, result);
                }
                return result;
            };
        }
    }
}
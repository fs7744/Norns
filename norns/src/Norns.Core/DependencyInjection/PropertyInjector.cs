using Norns.Extensions.Reflection.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Norns.Extensions.Reflection;

namespace Norns.DependencyInjection
{
    public class PropertyInjector : IDelegateServiceDefintionHandler
    {
        private static readonly MethodInfo getService = typeof(IServiceProvider).GetMethod("GetService");

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
                    defintion.Lifetime, CreatePropertyInjector(properties, defintion.ImplementationFactory));
            }
        }

        private Func<IServiceProvider, object> CreatePropertyInjector(PropertyInfo[] properties
            , Func<IServiceProvider, object> implementationFactory)
        {
            var dynamicMethod = new DynamicMethod($"invoker-{Guid.NewGuid()}", typeof(void), new Type[] { typeof(IServiceProvider), typeof(object) }, this.GetType().Module, true);
            var ilGen = dynamicMethod.GetILGenerator();
            foreach (var property in properties)
            {
                ilGen.EmitLoadArg(1);
                ilGen.EmitLoadArg(0);
                ilGen.EmitConstant(property.PropertyType, typeof(Type));
                ilGen.Emit(OpCodes.Callvirt, getService);
                ilGen.EmitConvertFromObject(property.PropertyType);
                ilGen.Emit(OpCodes.Callvirt, property.SetMethod);
            }
            ilGen.Emit(OpCodes.Ret);
            var setter = (Action<IServiceProvider, object>)dynamicMethod.CreateDelegate(typeof(Action<IServiceProvider, object>));
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
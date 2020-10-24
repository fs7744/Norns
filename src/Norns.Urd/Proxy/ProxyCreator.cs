using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Norns.Urd.Proxy
{
    public class ProxyCreator : IProxyCreator
    {
        const string GeneratedNameSpace = "Norns.Urd.DynamicGenerated";
        const string GeneratorAssemblyName = "Norns.Urd.DynamicGenerator";
        static readonly ConstructorInfo ObjectCtor = typeof(object).GetTypeInfo().DeclaredConstructors.Single();
        const MethodAttributes OverrideMethodAttributes = MethodAttributes.HideBySig | MethodAttributes.Virtual;
        private readonly ModuleBuilder moduleBuilder;
        private readonly Dictionary<string, Type> definedTypes = new Dictionary<string, Type>();
        private readonly object _lock = new object();

        public ProxyCreator()
        {
            var asmBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(GeneratorAssemblyName), AssemblyBuilderAccess.RunAndCollect);
            moduleBuilder = asmBuilder.DefineDynamicModule("core");
        }

        public Type CreateProxyType(Type serviceType, ProxyTypes proxyType = ProxyTypes.Inherit)
        {
            lock (_lock)
            {
                var name = GetProxyTypeName(serviceType, proxyType);
                if (!definedTypes.TryGetValue(name, out Type type))
                {
                    type = CreateProxyTypeInternal(name, serviceType, proxyType);
                    definedTypes[name] = type;
                }
                return type;
            }
        }

        private Type CreateProxyTypeInternal(string name, Type serviceType, ProxyTypes proxyType)
        {
                var (parent, interfaceTypes) = serviceType.IsInterface 
                    ? (typeof(object), new Type[] { serviceType})
                    : (serviceType, Type.EmptyTypes);
                var implTypeBuilder = moduleBuilder.DefineType(name, TypeAttributes.Public, parent, interfaceTypes);
                DefineEmptyConstructor(implTypeBuilder);
                if (proxyType == ProxyTypes.Facade)
                {
                    implTypeBuilder.DefineField("instance", serviceType, FieldAttributes.Private);
                }
                //var p = typeof(IFacadeProxy).GetProperty(nameof(IFacadeProxy.Instance));
                //var dp = implTypeBuilder.DefineProperty(p.Name, p.Attributes, p.PropertyType, Type.EmptyTypes);
                //if (p.CanRead)
                //{
                //    var method = MethodBuilderUtils.DefineClassMethod(p.GetMethod, implType, typeDesc);
                //    dp.SetGetMethod(method);
                //}
                //if (p.CanWrite)
                //{
                //    var method = MethodBuilderUtils.DefineClassMethod(p.SetMethod, implType, typeDesc);
                //    dp.SetSetMethod(method);
                //}
                return implTypeBuilder.CreateTypeInfo().AsType();
        }

        private string GetProxyTypeName(Type serviceType, ProxyTypes proxyType)
        {
            return $"{GeneratedNameSpace}.{serviceType.FullName}_Proxy_{proxyType}";
        }

        private void DefineEmptyConstructor(TypeBuilder typeBuilder)
        {
            var constructorBuilder = typeBuilder.DefineConstructor(MethodAttributes.Public, ObjectCtor.CallingConvention, Type.EmptyTypes);
            var ilGen = constructorBuilder.GetILGenerator();
            ilGen.EmitThis();
            ilGen.Emit(OpCodes.Call, ObjectCtor);
            ilGen.Emit(OpCodes.Ret);
        }

        //internal static MethodBuilder DefineClassMethod(MethodInfo method, Type implType, TypeDesc typeDesc)
        //{
        //    var attributes = OverrideMethodAttributes;

        //    if (method.Attributes.HasFlag(MethodAttributes.Public))
        //    {
        //        attributes = attributes | MethodAttributes.Public;
        //    }

        //    if (method.Attributes.HasFlag(MethodAttributes.Family))
        //    {
        //        attributes = attributes | MethodAttributes.Family;
        //    }

        //    if (method.Attributes.HasFlag(MethodAttributes.FamORAssem))
        //    {
        //        attributes = attributes | MethodAttributes.FamORAssem;
        //    }

        //    var methodBuilder = DefineMethod(method, method.Name, attributes, implType, typeDesc);
        //    return methodBuilder;
        //}
    }
}
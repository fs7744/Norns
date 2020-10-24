using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Norns.Urd.Proxy
{
    public abstract class AbstractProxyGenerator : IProxyGenerator
    {
        private const string GeneratedNameSpace = "Norns.Urd.DynamicGenerated";
        private static readonly ConstructorInfo ObjectCtor = typeof(object).GetTypeInfo().DeclaredConstructors.Single();
        private const MethodAttributes OverrideMethodAttributes = MethodAttributes.HideBySig | MethodAttributes.Virtual;
        public abstract ProxyTypes ProxyType { get; }

        public string GetProxyTypeName(Type serviceType)
        {
            return $"{GeneratedNameSpace}.{serviceType.FullName}_Proxy_{ProxyType}";
        }

        public virtual Type CreateProxyType(string proxyTypeName, Type serviceType, ModuleBuilder moduleBuilder)
        {
            var (parent, interfaceTypes) = serviceType.IsInterface
                    ? (typeof(object), new Type[] { serviceType })
                    : (serviceType, Type.EmptyTypes);
            var implTypeBuilder = moduleBuilder.DefineType(proxyTypeName, TypeAttributes.Public, parent, interfaceTypes);
            DefineConstructors(implTypeBuilder, serviceType);
            DefineFields(implTypeBuilder, serviceType);
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

        public virtual void DefineFields(TypeBuilder typeBuilder, Type serviceType)
        {
        }

        public virtual void DefineConstructors(TypeBuilder typeBuilder, Type serviceType)
        {
            DefineEmptyConstructor(typeBuilder);
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
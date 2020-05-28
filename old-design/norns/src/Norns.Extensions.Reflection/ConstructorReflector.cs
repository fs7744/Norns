using Norns.Extensions.Reflection.Extensions;
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Norns.Extensions.Reflection
{
    public class ConstructorReflector : Reflector<ConstructorInfo>
    {
        public ParameterReflector[] Parameters { get; }
        public Func<object[], object> Invoke { get; }

        public ConstructorReflector(ConstructorInfo member) : base(member, member.CustomAttributes)
        {
            Parameters = member.GetParameters().Select(x => x.GetReflector()).ToArray();
            Invoke = CreateInvoker();
        }

        public object InvokerFromService(IServiceProvider provider)
        {
            var parameters = Parameters.Select(i => provider.GetService(i.Member.ParameterType)).ToArray();
            return Invoke(parameters);
        }

        private Func<object[], object> CreateInvoker()
        {
            var dynamicMethod = new DynamicMethod($"invoker-{Guid.NewGuid()}", typeof(object), new Type[] { typeof(object[]) }, Member.Module, true);
            var ilGen = dynamicMethod.GetILGenerator();

            if (Parameters.Length == 0)
            {
                ilGen.Emit(OpCodes.Newobj, Member);
                return CreateDelegate();
            }
            var parameterTypes = Parameters.Select(i => i.Member.ParameterType).ToArray();
            var refParameterCount = parameterTypes.Count(x => x.IsByRef);
            if (refParameterCount == 0)
            {
                for (var i = 0; i < parameterTypes.Length; i++)
                {
                    ilGen.EmitLoadArg(0);
                    ilGen.EmitInt(i);
                    ilGen.Emit(OpCodes.Ldelem_Ref);
                    ilGen.EmitConvertFromObject(parameterTypes[i]);
                }
                ilGen.Emit(OpCodes.Newobj, Member);
                return CreateDelegate();
            }
            var indexedLocals = new IndexedLocalBuilder[refParameterCount];
            var index = 0;
            for (var i = 0; i < parameterTypes.Length; i++)
            {
                ilGen.EmitLoadArg(0);
                ilGen.EmitInt(i);
                ilGen.Emit(OpCodes.Ldelem_Ref);
                if (parameterTypes[i].IsByRef)
                {
                    var defType = parameterTypes[i].GetElementType();
                    var indexedLocal = new IndexedLocalBuilder(ilGen.DeclareLocal(defType), i);
                    indexedLocals[index++] = indexedLocal;
                    ilGen.EmitConvertFromObject(defType);
                    ilGen.Emit(OpCodes.Stloc, indexedLocal.LocalBuilder);
                    ilGen.Emit(OpCodes.Ldloca, indexedLocal.LocalBuilder);
                }
                else
                {
                    ilGen.EmitConvertFromObject(parameterTypes[i]);
                }
            }
            ilGen.Emit(OpCodes.Newobj, Member);
            for (var i = 0; i < indexedLocals.Length; i++)
            {
                ilGen.EmitLoadArg(0);
                ilGen.EmitInt(indexedLocals[i].Index);
                ilGen.Emit(OpCodes.Ldloc, indexedLocals[i].LocalBuilder);
                ilGen.EmitConvertToObject(indexedLocals[i].LocalType);
                ilGen.Emit(OpCodes.Stelem_Ref);
            }
            return CreateDelegate();

            Func<object[], object> CreateDelegate()
            {
                if (Member.DeclaringType.GetTypeInfo().IsValueType)
                {
                    ilGen.EmitConvertToObject(Member.DeclaringType);
                }

                ilGen.Emit(OpCodes.Ret);
                return (Func<object[], object>)dynamicMethod.CreateDelegate(typeof(Func<object[], object>));
            }
        }
    }
}
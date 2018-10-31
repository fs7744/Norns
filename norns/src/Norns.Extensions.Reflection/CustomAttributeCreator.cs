using System;
using System.Linq;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Norns.Extensions.Reflection.Extensions;

namespace Norns.Extensions.Reflection
{
    internal class CustomAttributeCreator
    {
        internal readonly HashSet<RuntimeTypeHandle> Tokens;
        private readonly Func<Attribute> invoker;

        public CustomAttributeCreator(CustomAttributeData customAttributeData)
        {
            Tokens = CreateTokens(customAttributeData.AttributeType);
            invoker = CreateInvoker(customAttributeData);
        }

        internal HashSet<RuntimeTypeHandle> CreateTokens(Type attributeType)
        {
            return new HashSet<RuntimeTypeHandle>(attributeType.GetAllBaseTypes().Select(i => i.TypeHandle));
        }

        private Func<Attribute> CreateInvoker(CustomAttributeData customAttributeData)
        {
            var dynamicMethod = new DynamicMethod($"invoker-{Guid.NewGuid()}", typeof(Attribute), null, customAttributeData.AttributeType.GetTypeInfo().Module, true);
            var ilGen = dynamicMethod.GetILGenerator();

            foreach (var constructorParameter in customAttributeData.ConstructorArguments)
            {
                if (constructorParameter.ArgumentType.IsArray)
                {
                    ilGen.EmitArray(((IEnumerable<CustomAttributeTypedArgument>)constructorParameter.Value).
                        Select(x => x.Value).ToArray(),
                        constructorParameter.ArgumentType.GetTypeInfo().UnWrapArrayType());
                }
                else
                {
                    ilGen.EmitConstant(constructorParameter.Value, constructorParameter.ArgumentType);
                }
            }
            var attributeType = customAttributeData.AttributeType;
            var attributeLocal = ilGen.DeclareLocal(attributeType);

            ilGen.EmitNew(customAttributeData.Constructor);

            ilGen.Emit(OpCodes.Stloc, attributeLocal);

            var attributeTypeInfo = attributeType.GetTypeInfo();

            foreach (var namedArgument in customAttributeData.NamedArguments)
            {
                ilGen.Emit(OpCodes.Ldloc, attributeLocal);
                if (namedArgument.TypedValue.ArgumentType.IsArray)
                {
                    ilGen.EmitArray(((IEnumerable<CustomAttributeTypedArgument>)namedArgument.TypedValue.Value).
                        Select(x => x.Value).ToArray(),
                        namedArgument.TypedValue.ArgumentType.GetTypeInfo().UnWrapArrayType());
                }
                else
                {
                    ilGen.EmitConstant(namedArgument.TypedValue.Value, namedArgument.TypedValue.ArgumentType);
                }
                if (namedArgument.IsField)
                {
                    var field = attributeTypeInfo.GetField(namedArgument.MemberName);
                    ilGen.Emit(OpCodes.Stfld, field);
                }
                else
                {
                    var property = attributeTypeInfo.GetProperty(namedArgument.MemberName);
                    ilGen.Emit(OpCodes.Callvirt, property.SetMethod);
                }
            }
            ilGen.Emit(OpCodes.Ldloc, attributeLocal);
            ilGen.Emit(OpCodes.Ret);
            return (Func<Attribute>)dynamicMethod.CreateDelegate(typeof(Func<Attribute>));
        }

        internal Attribute Create()
        {
            return invoker();
        }
    }
}
using Mono.Cecil;
using Mono.Cecil.Cil;
using Norns.AOP.Interceptors;
using Norns.StaticWeave;
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Norns.Test.StaticWeave
{
    public class AssemblyScannerTest
    {
        [Fact]
        public void FindNeedProxyClass()
        {
            var dllPath = Path.Combine(Directory.GetCurrentDirectory(), "debugdll", "TestFuncToDll.dll");
            var assembly = AssemblyDefinition.ReadAssembly(dllPath, new ReaderParameters() { ReadSymbols = false });
            var types = assembly.FindNeedInterceptTypes().ToArray();
            var typedReference = assembly.MainModule.TypeSystem.TypedReference;
            var contextReference = assembly.MainModule.ImportReference(typeof(InterceptContext));
            var additionsReference = assembly.MainModule.ImportReference(typeof(Dictionary<string, object>));
            var methodInfoReference = assembly.MainModule.ImportReference(typeof(System.Reflection.MethodInfo));
            var getTypeFromHandle = assembly.MainModule.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"));
            var getMethodInfo = assembly.MainModule.ImportReference(typeof(Type).GetMethod("GetMethod", new Type[] {  typeof(string), typeof(Type[])}));
            foreach (var t in types)
            {
                VariableDefinition typevariableDef = null;
                var staticCtor = t.FindStaticCtorMethod();
                if (staticCtor == null)
                {
                    staticCtor = new MethodDefinition(TypeReferenceExtensions.StaticCtorName, TypeReferenceExtensions.StaticCtorAttributes,
                        assembly.MainModule.TypeSystem.Void);
                    staticCtor.InsertBeforeLast(Instruction.Create(OpCodes.Ret));
                    t.Methods.Add(staticCtor);
                }
                var methods = t.FindNeedInterceptMethods().ToArray();

                foreach (var method in methods)
                {
                    var newMethodName = $"{method.Name}_{Guid.NewGuid()}";
                    var newfieldName = $"f_{newMethodName}";
                    var field = new FieldDefinition(newfieldName, FieldAttributes.Private | FieldAttributes.Static, methodInfoReference);
                    if (typevariableDef == null)
                    {
                        staticCtor.Body.InitLocals = true;
                        typevariableDef = new VariableDefinition(typedReference);
                        staticCtor.Body.Variables.Add(typevariableDef);
                        staticCtor.InsertBeforeLast(Instruction.Create(OpCodes.Ldtoken, t));
                        staticCtor.InsertBeforeLast(Instruction.Create(OpCodes.Call, getTypeFromHandle));
                        staticCtor.InsertBeforeLast(Instruction.Create(OpCodes.Stloc_S, typevariableDef));
                    }
                    staticCtor.InsertBeforeLast(Instruction.Create(OpCodes.Ldloc_S, typevariableDef));
                    staticCtor.InsertBeforeLast(Instruction.Create(OpCodes.Ldstr, method.Name));
                    staticCtor.InsertBeforeLast(Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)method.Parameters.Count));
                    staticCtor.InsertBeforeLast(Instruction.Create(OpCodes.Newarr, typedReference));
                    staticCtor.InsertBeforeLast(Instruction.Create(OpCodes.Callvirt, getMethodInfo));
                    staticCtor.InsertBeforeLast(Instruction.Create(OpCodes.Stsfld, field));
                    t.Fields.Add(field);

                    var newMethod = new MethodDefinition(newMethodName, method.Attributes, method.ReturnType);
                    newMethod.IsReuseSlot = method.IsReuseSlot;
                    newMethod.IsNewSlot = method.IsNewSlot;
                    newMethod.IsCheckAccessOnOverride = method.IsCheckAccessOnOverride;
                    newMethod.IsAbstract = method.IsAbstract;
                    newMethod.IsSpecialName = method.IsSpecialName;
                    newMethod.IsPInvokeImpl = method.IsPInvokeImpl;
                    newMethod.IsUnmanagedExport = method.IsUnmanagedExport;
                    newMethod.IsRuntimeSpecialName = method.IsRuntimeSpecialName;
                    newMethod.HasSecurity = method.HasSecurity;
                    newMethod.IsIL = method.IsIL;
                    newMethod.IsNative = method.IsNative;
                    newMethod.IsRuntime = method.IsRuntime;
                    newMethod.IsUnmanaged = method.IsUnmanaged;
                    newMethod.IsHideBySig = method.IsHideBySig;
                    newMethod.IsManaged = method.IsManaged;
                    newMethod.IsPreserveSig = method.IsPreserveSig;
                    newMethod.IsInternalCall = method.IsInternalCall;
                    newMethod.IsSynchronized = method.IsSynchronized;
                    newMethod.NoInlining = method.NoInlining;
                    newMethod.NoOptimization = method.NoOptimization;
                    newMethod.AggressiveInlining = method.AggressiveInlining;
                    newMethod.IsSetter = method.IsSetter;
                    newMethod.IsGetter = method.IsGetter;
                    newMethod.IsOther = method.IsOther;
                    newMethod.IsAddOn = method.IsAddOn;
                    newMethod.IsRemoveOn = method.IsRemoveOn;
                    newMethod.IsFire = method.IsFire;
                    newMethod.DeclaringType = method.DeclaringType;
                    newMethod.IsForwardRef = method.IsForwardRef;
                    newMethod.IsVirtual = method.IsVirtual;
                    newMethod.IsStatic = method.IsStatic;
                    newMethod.Attributes = method.Attributes;
                    newMethod.ImplAttributes = method.ImplAttributes;
                    newMethod.SemanticsAttributes = method.SemanticsAttributes;
                    newMethod.Body = method.Body;
                    newMethod.IsFinal = method.IsFinal;
                    newMethod.IsCompilerControlled = method.IsCompilerControlled;
                    newMethod.IsPrivate = method.IsPrivate;
                    newMethod.IsFamilyAndAssembly = method.IsFamilyAndAssembly;
                    newMethod.IsAssembly = method.IsAssembly;
                    newMethod.IsFamily = method.IsFamily;
                    newMethod.IsFamilyOrAssembly = method.IsFamilyOrAssembly;
                    newMethod.IsPublic = method.IsPublic;
                    newMethod.PInvokeInfo = method.PInvokeInfo;
                    newMethod.MethodReturnType = method.MethodReturnType;
                    newMethod.ReturnType = method.ReturnType;
                    newMethod.CallingConvention = method.CallingConvention;
                    newMethod.ExplicitThis = method.ExplicitThis;
                    newMethod.HasThis = method.HasThis;
                    newMethod.MetadataToken = method.MetadataToken;
                    t.Methods.Add(newMethod);

                    var ct = contextReference.Resolve();
                    var callMethod = new MethodDefinition($"Call_{newMethodName}", MethodAttributes.Private, assembly.MainModule.TypeSystem.Void);
                    var contextParameter = new ParameterDefinition("context", ParameterAttributes.None, contextReference);
                    callMethod.Parameters.Add(contextParameter);
                    var il = callMethod.Body.GetILProcessor();
                    if (method.ReturnType != assembly.MainModule.TypeSystem.Void)
                    {
                        il.Append(Instruction.Create(OpCodes.Ldarg_1));
                    }
                    il.Append(Instruction.Create(OpCodes.Ldarg_0));
                    foreach (var parameter in method.Parameters)
                    {
                        il.Append(Instruction.Create(OpCodes.Ldarg_1));
                        il.Append(Instruction.Create(OpCodes.Ldfld, ct.Fields.First(j => j.Name == "Parameters")));
                        il.Append(Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)parameter.Index));
                        il.Append(Instruction.Create(OpCodes.Ldelem_Ref, parameter.Index));
                        if (parameter.ParameterType.IsValueType)
                        {
                            il.Append(Instruction.Create(OpCodes.Unbox_Any, parameter.ParameterType));
                        }
                    }
                    il.Append(Instruction.Create(OpCodes.Call, newMethod));
                    if (method.ReturnType != assembly.MainModule.TypeSystem.Void)
                    {
                        if (method.ReturnType.IsValueType)
                        {
                            il.Append(Instruction.Create(OpCodes.Box, method.ReturnType));
                        }
                        il.Append(Instruction.Create(OpCodes.Stsfld, ct.Fields.First(j => j.Name == "Result")));
                    }
                    il.Append(Instruction.Create(OpCodes.Ret));
                    var field1 = new FieldDefinition($"f_{callMethod.Name}", FieldAttributes.Private, assembly.MainModule.ImportReference(typeof(InterceptDelegate)));
                    t.Fields.Add(field1);

                    method.Body = new MethodBody(method);
                    method.Body.InitLocals = true;
                    var context = new VariableDefinition(contextReference);
                    var parameters = new VariableDefinition(assembly.MainModule.ImportReference(typeof(object[])));
                    method.Body.Variables.Add(context);
                    il = method.Body.GetILProcessor();
                    il.Append(Instruction.Create(OpCodes.Initobj, contextReference));
                    il.Append(Instruction.Create(OpCodes.Stloc_S, context));
                    il.Append(Instruction.Create(OpCodes.Ldloc_S, context));
                    il.Append(Instruction.Create(OpCodes.Ldfld, field));
                    il.Append(Instruction.Create(OpCodes.Stsfld, ct.Fields.First(i => i.Name == "ServiceMethod")));
                    il.Append(Instruction.Create(OpCodes.Ldloc_S, context));
                    il.Append(Instruction.Create(OpCodes.Newobj, additionsReference.Resolve().Methods.First(i => i.Name == TypeReferenceExtensions.CtorName)));
                    il.Append(Instruction.Create(OpCodes.Stsfld, ct.Fields.First(i => i.Name == "Additions")));
                    il.Append(Instruction.Create(OpCodes.Ldloc_S, context));
                    il.Append(Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)method.Parameters.Count));
                    il.Append(Instruction.Create(OpCodes.Newarr, assembly.MainModule.TypeSystem.Object));
                    il.Append(Instruction.Create(OpCodes.Stloc_S, parameters));
                    il.Append(Instruction.Create(OpCodes.Ldloc_S, parameters));
                    il.Append(Instruction.Create(OpCodes.Stsfld, ct.Fields.First(i => i.Name == "Parameters")));
                    foreach (var parameter in method.Parameters)
                    {
                        il.Append(Instruction.Create(OpCodes.Ldloc_S, parameters));
                        il.Append(Instruction.Create(OpCodes.Ldc_I4_S, (sbyte)parameter.Index));
                        il.Append(Instruction.Create(OpCodes.Ldarga_S, parameter.Index));
                        if (parameter.ParameterType.IsValueType)
                        {
                            il.Append(Instruction.Create(OpCodes.Box, parameter.ParameterType));
                        }
                        il.Append(Instruction.Create(OpCodes.Stelem_Ref));
                    }

                    il.Append(Instruction.Create(OpCodes.Ldarg_0));
                    il.Append(Instruction.Create(OpCodes.Ldfld, field1));
                    il.Append(Instruction.Create(OpCodes.Ldloc_S, context));
                    il.Append(Instruction.Create(OpCodes.Callvirt, assembly.MainModule.ImportReference(typeof(InterceptDelegate)).Resolve().Methods.First(j => j.Name == "Invoke")));
                    il.Append(Instruction.Create(OpCodes.Ret));
                }
            }
        }
    }
}
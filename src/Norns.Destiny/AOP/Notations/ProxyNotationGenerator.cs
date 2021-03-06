﻿using Norns.Destiny.Notations;
using Norns.Destiny.Structure;
using Norns.Destiny.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.AOP.Notations
{
    public class ProxyNotationGenerator : AbstractNotationGenerator
    {
        private readonly IEnumerable<IInterceptorGenerator> interceptors;
        private readonly Func<ITypeSymbolInfo, bool> filterProxy;

        public ProxyNotationGenerator(IEnumerable<IInterceptorGenerator> interceptors, Func<ITypeSymbolInfo, bool> filterProxy)
        {
            this.filterProxy = filterProxy;
            this.interceptors = interceptors;
        }

        public override bool Filter(ITypeSymbolInfo type)
        {
            return filterProxy(type);
        }

        public override INotation CreateImplement(ITypeSymbolInfo type)
        {
            var @namespace = new NamespaceNotation() { Name = type.Namespace };
            var @class = new ClassNotation()
            {
                Accessibility = type.Accessibility,
                Name = $"Proxy{type.Name}{RandomUtils.NewName()}"
            };
            @class.CustomAttributes.Add($"[Norns.Destiny.Attributes.Proxy(typeof({CreateImplementKey(type)}))]".ToNotation());
            if (type.IsGenericType)
            {
                @class.TypeParameters.AddRange(type.TypeParameters.Select(i => i.ToNotation()));
            }
            @namespace.Members.Add(@class);
            @class.Inherits.Add(type.FullName.ToNotation());
            var context = new ProxyGeneratorContext()
            {
                Symbol = type
            };
            context.SetCurrentNamespaceNotation(@namespace);
            context.SetCurrentClassNotation(@class);
            foreach (var member in type.Members.Union(type.Interfaces.SelectMany(i => i.Members)).Distinct())
            {
                switch (member)
                {
                    case IMethodSymbolInfo method when !method.IsStatic 
                        && method.MethodKind != MethodKindInfo.PropertyGet
                        && method.MethodKind != MethodKindInfo.PropertySet
                        && (method.CanOverride() || (method.ContainingType.IsInterface && !method.IsAbstract))
                        && method.Name != "Finalize":
                        @class.Members.Add(CreateProxyMethod(method, context));
                        break;

                    case IMethodSymbolInfo method when !method.IsStatic && method.MethodKind == MethodKindInfo.Constructor:
                        @class.Members.Add(GenerateImplementConstructor(method, @class.Name));
                        break;

                    case IPropertySymbolInfo property when property.CanOverride():
                        @class.Members.Add(CreateProxyProperty(property, context));
                        break;

                    default:
                        break;
                }
            }
            AddProxyInfo(@class, context, type);
            return @namespace;
        }

        private void AddProxyInfo(ClassNotation @class, ProxyGeneratorContext context, ITypeSymbolInfo type)
        {
            @class.Members.Add(new FieldNotation() { Accessibility = AccessibilityInfo.Public, Type = type.FullName, Name = context.GetProxyFieldName() });
            var setProxyNode = new MethodNotation() { Accessibility = AccessibilityInfo.Public, ReturnType = "void", Name = "SetProxy" };
            setProxyNode.Parameters.Add(new ParameterNotation() { Type = "object", Name = "instance" });
            setProxyNode.Parameters.Add(new ParameterNotation() { Type = "System.IServiceProvider", Name = "serviceProvider" });
            setProxyNode.Body.AddRange(Notation.Create(context.GetProxyFieldName(), " = instance as ", type.FullName, ";"));
            @class.Members.Add(setProxyNode);
            @class.Inherits.Add("Norns.Destiny.AOP.IInterceptProxy".ToNotation());
            foreach (var f in @class.Members.Select(i => i as FieldNotation).Where(i => i != null && i.IsFromDI))
            {
                setProxyNode.Body.AddRange(Notation.Create(f.Name, " = serviceProvider.GetService(typeof(", f.Type, ")) as ", f.Type, ";"));
            }
        }

        private ConstructorNotation GenerateImplementConstructor(IMethodSymbolInfo method, string className)
        {
            var notation = method.ToConstructorNotation(className);
            notation.Accessibility = AccessibilityInfo.Public;
            notation.HasBase = true;
            return notation;
        }

        private INotation CreateProxyProperty(IPropertySymbolInfo property, ProxyGeneratorContext typeContext)
        {
            var context = new ProxyGeneratorContext()
            {
                Parent = typeContext,
                Symbol = property
            };
            PropertyNotation notation;
            List<INotation> callName = new List<INotation>();
            if (property.IsIndexer)
            {
                var indexer = new IndexerPropertyNotation();
                indexer.Parameters.AddRange(property.Parameters.Select(i => new ParameterNotation()
                {
                    Type = i.Type.FullName,
                    Name = i.Name
                }));
                notation = indexer;
                callName.Add(ConstNotations.OpenBracket);
                callName.Add(indexer.Parameters.ToCallParameters());
                callName.Add(ConstNotations.CloseBracket);
            }
            else
            {
                callName.Add(ConstNotations.Dot);
                callName.Add(property.Name.ToNotation());
                notation = new PropertyNotation();
            }
            var isInterface = property.ContainingType.IsInterface;
            notation.IsOverride = !isInterface && property.CanOverride();
            notation.Accessibility = property.Accessibility;
            notation.Name = property.Name;
            notation.Type = property.Type.FullName;
            if (property.CanRead)
            {
                context.SetCurrentPropertyMethod(property.GetMethod);
                var getter = PropertyMethodNotation.Create(true);
                getter.Accessibility = property.GetMethod.Accessibility;
                var returnValueParameterName = context.GetReturnValueParameterName();
                getter.Body.AddRange(Notation.Create("var ", returnValueParameterName, " = default(", property.Type.FullName, ");"));
                getter.Body.AddRange(interceptors.SelectMany(i => i.BeforeMethod(context)));
                if (isInterface || (getter.Accessibility == AccessibilityInfo.Public || getter.Accessibility == AccessibilityInfo.Internal))
                {
                    getter.Body.AddRange(Notation.Create(returnValueParameterName, " = ", context.GetProxyFieldName()));
                    getter.Body.AddRange(callName);
                    getter.Body.Add(ConstNotations.Semicolon);
                }
                getter.Body.AddRange(interceptors.SelectMany(i => i.AfterMethod(context)));
                getter.Body.AddRange(Notation.Create("return ", returnValueParameterName, ";"));
                notation.Accessers.Add(getter);
            }
            if (property.CanWrite)
            {
                context.SetCurrentPropertyMethod(property.SetMethod);
                var setter = PropertyMethodNotation.Create(false);
                setter.Accessibility = property.SetMethod.Accessibility;
                var returnValueParameterName = context.GetReturnValueParameterName();
                setter.Body.AddRange(Notation.Create("var ", returnValueParameterName, " = value;"));
                setter.Body.AddRange(interceptors.SelectMany(i => i.BeforeMethod(context)));
                if (isInterface || (setter.Accessibility == AccessibilityInfo.Public || setter.Accessibility == AccessibilityInfo.Internal))
                {
                    setter.Body.Add(context.GetProxyFieldName().ToNotation());
                    setter.Body.AddRange(callName);
                    setter.Body.AddRange(Notation.Create(" = ", returnValueParameterName, ";"));
                }
                setter.Body.AddRange(interceptors.SelectMany(i => i.AfterMethod(context)));
                notation.Accessers.Add(setter);
            }
            return notation;
        }

        private INotation CreateProxyMethod(IMethodSymbolInfo method, ProxyGeneratorContext typeContext)
        {
            var context = new ProxyGeneratorContext()
            {
                Parent = typeContext,
                Symbol = method
            };

            var notation = method.ToNotationDefinition();
            context.SetCurrentMethodNotation(notation);
            var isInterface = method.ContainingType.IsInterface;
            notation.IsOverride = !isInterface && method.CanOverride();
            var returnValueParameterName = context.GetReturnValueParameterName();
            if (method.HasReturnValue)
            {
                notation.Body.AddRange(Notation.Create("var ", returnValueParameterName, " = default(", method.IsAsync ? method.ReturnType.TypeArguments.First().FullName : method.ReturnType.FullName, ");"));
            }
            notation.Body.Add(method.Parameters.Where(i => i.RefKind == RefKindInfo.Out).Select(i => $"{i.Name} = default;".ToNotation()).Combine());
            notation.Body.AddRange(interceptors.SelectMany(i => i.BeforeMethod(context)));
            if (isInterface || (!method.IsAbstract && (method.Accessibility == AccessibilityInfo.Internal || method.Accessibility == AccessibilityInfo.Public)))
            {
                if (method.HasReturnValue)
                {
                    notation.Body.AddRange(Notation.Create(returnValueParameterName, " = "));
                }
                notation.Body.AddRange(Notation.Create(method.IsAsync ? "await " : string.Empty, method.ContainingType.IsInterface && !method.IsAbstract ? $"({context.GetProxyFieldName()} as {method.ContainingType.FullName})"  : context.GetProxyFieldName(), ".", method.Name));
                notation.Body.Add(ConstNotations.OpenParen);
                notation.Body.Add(notation.Parameters.ToCallParameters());
                notation.Body.Add(ConstNotations.CloseParen);
                notation.Body.Add(ConstNotations.Semicolon);
            }
            notation.Body.AddRange(interceptors.SelectMany(i => i.AfterMethod(context)));

            if (method.HasReturnValue)
            {
                notation.Body.AddRange(Notation.Create("return ", returnValueParameterName, ";"));
            }
            return notation;
        }
    }
}
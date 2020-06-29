using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.Notations;
using Norns.Destiny.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.AOP.Notations
{
    public class ProxyNotationGenerator : AbstractNotationGenerator
    {
        private readonly IEnumerable<IInterceptorGenerator> interceptors;

        public ProxyNotationGenerator(IEnumerable<IInterceptorGenerator> interceptors)
        {
            this.interceptors = interceptors;
        }

        public override bool Filter(ITypeSymbolInfo type)
        {
            return true;
        }

        public override INotation CreateImplement(ITypeSymbolInfo type)
        {
            var @namespace = new NamespaceNotation() { Name = type.Namespace };
            var @class = new ClassNotation()
            {
                Accessibility = type.Accessibility,
                Name = $"Proxy{type.Name}{RandomUtils.NewName()}"
            };
            @class.CustomAttributes.Add($"[Norns.Destiny.Attributes.Proxy(typeof({(type.IsGenericType ? type.GenericDefinitionName : type.FullName)}))]".ToNotation());
            @namespace.Members.Add(@class);
            @class.Inherits.Add(type.FullName.ToNotation());
            var context = new ProxyGeneratorContext()
            {
                Symbol = type
            };
            context.SetCurrentNamespaceNotation(@namespace);
            context.SetCurrentClassNotation(@class);
            foreach (var member in type.GetMembers().Union(type.GetInterfaces().SelectMany(i => i.GetMembers())).Distinct())
            {
                switch (member)
                {
                    case IMethodSymbolInfo method when method.MethodKind != MethodKindInfo.PropertyGet
                        && method.MethodKind != MethodKindInfo.PropertySet
                        && method.CanOverride():
                        @class.Members.Add(CreateProxyMethod(method, context));
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

        private INotation CreateProxyProperty(IPropertySymbolInfo property, ProxyGeneratorContext typeContext)
        {
            var context = new ProxyGeneratorContext()
            {
                Parent = typeContext,
                Symbol = property
            };
            PropertyNotation notation;
            if (property.IsIndexer)
            {
                var indexer = new IndexerPropertyNotation();
                indexer.Parameters.AddRange(property.Parameters.Select(i => new ParameterNotation()
                {
                    Type = i.Type.FullName,
                    Name = i.Name
                }));
                notation = indexer;
            }
            else
            {
                notation = new PropertyNotation();
            }
            notation.IsOverride = true;
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
                getter.Body.AddRange(Notation.Create(returnValueParameterName, " = ", context.GetProxyFieldName(), ".", property.Name, ";"));
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
                setter.Body.AddRange(Notation.Create(context.GetProxyFieldName(), ".", property.Name, " = ", returnValueParameterName, ";"));
                setter.Body.AddRange(interceptors.SelectMany(i => i.AfterMethod(context)));
                notation.Accessers.Add(setter);
            }
            return notation;
        }

        private INotation CreateProxyMethod(IMethodSymbolInfo method, ProxyGeneratorContext typeContext)
        {
            var context = new ProxyGeneratorContext()
            {
                Parent = typeContext
            };

            var notation = method.ToNotationDefinition();
            context.SetCurrentMethodNotation(notation);
            notation.IsOverride = true;
            var returnValueParameterName = context.GetReturnValueParameterName();
            if (method.HasReturnValue)
            {
                notation.Body.AddRange(Notation.Create("var ", returnValueParameterName, " = default(", method.IsAsync ? method.ReturnType.TypeParameters.First().FullName : method.ReturnType.FullName, ");"));
            }

            notation.Body.AddRange(interceptors.SelectMany(i => i.BeforeMethod(context)));
            if (!method.IsAbstract)
            {
                if (method.HasReturnValue)
                {
                    notation.Body.AddRange(Notation.Create(returnValueParameterName, " = "));
                }
                notation.Body.AddRange(Notation.Create(method.IsAsync ? "await " : string.Empty, context.GetProxyFieldName(), ".", method.Name));
                notation.Body.Add(notation.Parameters.ToCallParameters());
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
using Norns.Destiny.Abstraction.Coder;
using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.Notations;
using Norns.Destiny.Utils;
using System.Collections.Generic;
using System.Linq;

namespace Norns.Destiny.AOP.Notations
{
    public class ProxyNotationGenerator : INotationGenerator
    {
        private readonly IEnumerable<IInterceptorGenerator> interceptors;

        public ProxyNotationGenerator(IEnumerable<IInterceptorGenerator> interceptors)
        {
            this.interceptors = interceptors;
        }

        public INotation GenerateNotations(ISymbolSource source)
        {
            return source.GetTypes()
                .Select(CreateProxy)
                .Combine();
        }

        private INotation CreateProxy(ITypeSymbolInfo type)
        {
            var @namespace = new NamespaceNotation() { Name = type.Namespace };
            var @class = new ClassNotation()
            {
                Accessibility = type.Accessibility,
                Name = $"Proxy{type.Name}{RandomUtils.NewName()}"
            };
            @class.CustomAttributes.Add($"[Norns.Destiny.Attributes.Proxy(typeof({type.FullName}))]".ToNotation());
            @namespace.Members.Add(@class);
            @class.Inherits.Add(type.FullName.ToNotation());
            var context = new ProxyGeneratorContext()
            {
                Symbol = type
            };
            foreach (var member in type.GetMembers().Union(type.GetInterfaces().SelectMany(i => i.GetMembers())).Distinct())
            {
                switch (member)
                {
                    case IMethodSymbolInfo method when method.MethodKind != MethodKindInfo.PropertyGet
                        && method.MethodKind != MethodKindInfo.PropertySet
                        && (method.IsAbstract || method.IsVirtual || method.IsOverride):
                        @class.Members.Add(CreateProxyMethod(method, context));
                        break;

                    case IPropertySymbolInfo property when property.IsAbstract || property.IsVirtual || property.IsOverride:
                        @class.Members.Add(CreateProxyProperty(property, context));
                        break;

                    default:
                        break;
                }
            }
            return @namespace;
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
                getter.Body.AddRange(interceptors.Select(i => i.BeforeMethod(context)));
                getter.Body.AddRange(Notation.Create(returnValueParameterName, " = ", context.GetProxyFieldName(), ".", property.Name, ";"));
                getter.Body.AddRange(interceptors.Select(i => i.AfterMethod(context)));
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
                setter.Body.AddRange(interceptors.Select(i => i.BeforeMethod(context)));
                setter.Body.AddRange(Notation.Create(context.GetProxyFieldName(), ".", property.Name, " = ", returnValueParameterName, ";"));
                setter.Body.AddRange(interceptors.Select(i => i.AfterMethod(context)));
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
            var notation = new MethodNotation()
            {
                Accessibility = method.Accessibility,
                ReturnType = method.ReturnType.FullName,
                Name = method.Name
            };
            notation.IsOverride = true;
            notation.IsAsync = method.IsAsync;
            notation.Parameters.AddRange(method.Parameters.Select(i => new ParameterNotation()
            {
                Type = i.Type.FullName,
                Name = i.Name
            }));
            var returnValueParameterName = context.GetReturnValueParameterName();
            if (method.HasReturnValue)
            {
                notation.Body.AddRange(Notation.Create("var ", returnValueParameterName, " = default(", method.IsAsync ? method.ReturnType.TypeParameters.First().FullName : method.ReturnType.FullName, ");"));
            }

            notation.Body.AddRange(interceptors.Select(i => i.BeforeMethod(context)));
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
            notation.Body.AddRange(interceptors.Select(i => i.AfterMethod(context)));

            if (method.HasReturnValue)
            {
                notation.Body.AddRange(Notation.Create("return ", returnValueParameterName, ";"));
            }
            return notation;
        }
    }
}
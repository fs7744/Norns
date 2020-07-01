using Norns.Destiny.Abstraction.Structure;
using Norns.Destiny.Notations;
using Norns.Destiny.Utils;
using System;
using System.Linq;

namespace Norns.Destiny.AOP.Notations
{
    public class DefaultImplementNotationGenerator : AbstractNotationGenerator
    {
        private readonly Func<ITypeSymbolInfo, bool> filter;

        public DefaultImplementNotationGenerator(Func<ITypeSymbolInfo, bool> filter)
        {
            this.filter = filter;
        }

        public override bool Filter(ITypeSymbolInfo type)
        {
            return filter(type);
        }

        public override INotation CreateImplement(ITypeSymbolInfo type)
        {
            var context = new ProxyGeneratorContext()
            {
                Symbol = type
            };
            var @namespace = new NamespaceNotation() { Name = type.Namespace };
            var @class = new ClassNotation()
            {
                Accessibility = type.Accessibility,
                Name = $"DefaultImplement{type.Name}{RandomUtils.NewName()}"
            };
            @class.CustomAttributes.Add($"[Norns.Destiny.Attributes.DefaultImplement(typeof({(type.IsGenericType ? type.GenericDefinitionName : type.FullName)}))]".ToNotation());
            if (type.IsGenericType)
            {
                @class.TypeParameters.AddRange(type.TypeParameters.Select(i => i.ToNotation()));
            }
            @namespace.Members.Add(@class);
            @class.Inherits.Add(type.FullName.ToNotation());
            foreach (var member in type.GetMembers().Union(type.GetInterfaces().SelectMany(i => i.GetMembers())).Distinct())
            {
                switch (member)
                {
                    case IMethodSymbolInfo method when method.IsAbstract && method.MethodKind == MethodKindInfo.Method:
                        @class.Members.Add(GenerateImplementMethod(method, type.IsInterface, context));
                        break;

                    case IMethodSymbolInfo method when method.MethodKind == MethodKindInfo.Constructor:
                        @class.Members.Add(GenerateImplementConstructor(method, @class.Name));
                        break;

                    case IPropertySymbolInfo property:
                        @class.Members.Add(GenerateImplementProperty(property, type.IsInterface, context));
                        break;

                    default:
                        break;
                }
            }
            return @namespace;
        }

        private INotation GenerateImplementConstructor(IMethodSymbolInfo method, string className)
        {
            var notation = method.ToConstructorNotation(className);
            notation.Accessibility = AccessibilityInfo.Public;
            notation.HasBase = true;
            return notation;
        }

        private INotation GenerateImplementMethod(IMethodSymbolInfo method, bool isInterface, ProxyGeneratorContext typeContext)
        {
            var context = new ProxyGeneratorContext()
            {
                Parent = typeContext,
                Symbol = method
            };
            var notation = method.ToNotationDefinition();
            context.SetCurrentMethodNotation(notation);
            notation.IsOverride = !isInterface && method.CanOverride();
            notation.Body.Add(method.Parameters.Where(i => i.RefKind == RefKindInfo.Out).Select(i => $"{i.Name} = default;".ToNotation()).Combine());
            var returnValueParameterName = context.GetReturnValueParameterName();
            if (method.HasReturnValue)
            {
                notation.Body.AddRange(Notation.Create("var ", returnValueParameterName, " = default(", method.IsAsync ? method.ReturnType.TypeArguments.First().FullName : method.ReturnType.FullName, ");"));
            }
            if (method.HasReturnValue)
            {
                notation.Body.AddRange(Notation.Create("return ", returnValueParameterName, ";"));
            }
            return notation;
        }

        private INotation GenerateImplementProperty(IPropertySymbolInfo property, bool isInterface, ProxyGeneratorContext typeContext)
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
                getter.Body.AddRange(Notation.Create("return ", returnValueParameterName, ";"));
                notation.Accessers.Add(getter);
            }
            if (property.CanWrite)
            {
                context.SetCurrentPropertyMethod(property.SetMethod);
                var setter = PropertyMethodNotation.Create(false);
                setter.Accessibility = property.SetMethod.Accessibility;
                setter.Body.Add(ConstNotations.Blank);
                notation.Accessers.Add(setter);
            }
            return notation;
        }
    }
}
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
                    case IMethodSymbolInfo method when method.IsAbstract && method.MethodKind != MethodKindInfo.PropertyGet && method.MethodKind != MethodKindInfo.PropertySet:
                        @class.Members.Add(GenerateImplementMethod(method));
                        break;

                    case IPropertySymbolInfo property:
                        @class.Members.Add(GenerateImplementProperty(property));
                        break;

                    default:
                        break;
                }
            }
            return @namespace;
        }

        private INotation GenerateImplementMethod(IMethodSymbolInfo method)
        {
            var notation = method.ToNotationDefinition();
            notation.Body.Add(method.Parameters.Where(i => i.RefKind == RefKindInfo.Out).Select(i => $"{i.Name} = default;".ToNotation()).Combine());
            if (method.HasReturnValue)
            {
                notation.Body.Add("return default;".ToNotation());
            }
            return notation;
        }

        private INotation GenerateImplementProperty(IPropertySymbolInfo property)
        {
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
            notation.Accessibility = property.Accessibility;
            notation.Name = property.Name;
            notation.Type = property.Type.FullName;
            if (property.CanRead)
            {
                var getter = PropertyMethodNotation.Create(true);
                getter.Accessibility = property.GetMethod.Accessibility;
                getter.Body.AddRange(Notation.Create("return default(", property.Type.FullName, ");"));
                notation.Accessers.Add(getter);
            }
            if (property.CanWrite)
            {
                var setter = PropertyMethodNotation.Create(false);
                setter.Accessibility = property.SetMethod.Accessibility;
                setter.Body.Add(ConstNotations.Blank);
                notation.Accessers.Add(setter);
            }
            return notation;
        }
    }
}
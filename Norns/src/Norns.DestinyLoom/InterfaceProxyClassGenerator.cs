using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Norns.DestinyLoom
{
    public class InterfaceProxyClassGenerator : AbstractProxyClassGenerator
    {
        public InterfaceProxyClassGenerator(IInterceptorGenerator[] interceptors) : base(interceptors)
        {
        }

        public override bool CanProxy(INamedTypeSymbol type)
        {
            return @type.TypeKind == TypeKind.Interface;
        }

        public override string GenerateProxyClass(ProxyGeneratorContext context)
        {
            var @namespace = new NamespaceNode($"{context.Type.ContainingNamespace.ToDisplayString()}.Proxy{GuidHelper.NewGuidName()}");
            var @class = new ClassNode($"Proxy{context.Type.Name}{GuidHelper.NewGuidName()}");
            @class.Accessibility = context.Type.DeclaredAccessibility.ToString().ToLower();
            @class.CustomAttributes.Add("[Norns.Fate.Abstraction.Proxy(typeof(");
            @class.CustomAttributes.Add(context.Type.ToDisplayString());
            @class.CustomAttributes.Add("))]");
            @class.Fields.Add(new FieldNode()
            {
                Accessibility = "public",
                Type = context.Type.ToDisplayString(),
                Name = context.ProxyFieldName,
            });
            var setProxyNode = new MethodNode()
            {
                Accessibility = "public",
                Return = "void",
                Name = "SetProxy",
            };
            setProxyNode.Parameters.Add(new ParameterNode() { Name = "instance", Type = "object" });
            setProxyNode.Parameters.Add(new ParameterNode() { Name = "serviceProvider", Type = "System.IServiceProvider" });
            setProxyNode.Body.Add(context.ProxyFieldName);
            setProxyNode.Body.Add(" = instance as ");
            setProxyNode.Body.Add(context.Type.ToDisplayString());
            setProxyNode.Body.Add(";");
            @class.Methods.Add(setProxyNode);
            @namespace.Classes.Add(@class);
            @class.Inherit.Types.Add(context.Type.ToDisplayString());
            @class.Inherit.Types.Add("Norns.Fate.Abstraction.IInterceptProxy");
            foreach (var member in context.Type.GetMembers())
            {
                switch (member)
                {
                    case IMethodSymbol method when method.MethodKind != MethodKind.PropertyGet && method.MethodKind != MethodKind.PropertySet:
                        var methodGeneratorContext = new ProxyMethodGeneratorContext(method, context);
                        var m = GenerateProxyMethod(methodGeneratorContext);
                        @class.Methods.Add(m);
                        break;

                    case IPropertySymbol property:
                        var propertyGeneratorContext = new ProxyPropertyGeneratorContext(property, context);
                        @class.Properties.Add(GenerateProxyProperty(propertyGeneratorContext));
                        break;

                    default:
                        break;
                }
            }
            var sb = new StringBuilder();
            @namespace.Generate(sb);
            return sb.ToString();
        }

        private PropertyNode GenerateProxyProperty(ProxyPropertyGeneratorContext propertyGeneratorContext)
        {
            var p = propertyGeneratorContext.Property;
            var node = new PropertyNode()
            {
                Accessibility = p.DeclaredAccessibility.ToString().ToLower(),
                Type = p.Type.ToDisplayString(),
                Name = p.IsIndexer ? p.Name.Replace("]", string.Join(",", p.Parameters.Select(i => i.Type.ToDisplayString() + " " + i.Name)) + "]") : p.Name
            };

            if (!p.IsWriteOnly)
            {
                node.Getter = new PropertyMethodNode()
                {
                    Name = "get",
                    Accessibility = p.GetMethod.DeclaredAccessibility.ToString().ToLower(),
                };
                if (node.Accessibility == node.Getter.Accessibility)
                {
                    node.Getter.Accessibility = string.Empty;
                }
                var getterContext = new ProxyMethodGeneratorContext(p.GetMethod, propertyGeneratorContext.ClassGeneratorContext);
                node.Getter.Body.Add("var ");
                node.Getter.Body.Add(getterContext.ReturnValueParameterName);
                node.Getter.Body.Add(" = default");
                node.Getter.Body.Add("(");
                node.Getter.Body.Add(p.Type.ToDisplayString());
                node.Getter.Body.Add(");");
                foreach (var item in interceptors)
                {
                    node.Getter.Body.AddRange(item.BeforeMethod(getterContext));
                }
                node.Getter.Body.Add(getterContext.ReturnValueParameterName);
                node.Getter.Body.Add(" = ");
                node.Getter.Body.Add(propertyGeneratorContext.ClassGeneratorContext.ProxyFieldName);
                if (p.IsIndexer)
                {
                    node.Getter.Body.Add("[");
                    node.Getter.Body.Add(string.Join(",", p.Parameters.Select(i => i.Name)));
                    node.Getter.Body.Add("]");
                }
                else
                {
                    node.Getter.Body.Add(".");
                    node.Getter.Body.Add(p.Name);
                }
                node.Getter.Body.Add(";");
                foreach (var item in interceptors)
                {
                    node.Getter.Body.AddRange(item.AfterMethod(getterContext));
                }
                node.Getter.Body.Add("return ");
                node.Getter.Body.Add(getterContext.ReturnValueParameterName);
                node.Getter.Body.Add("; ");
            }
            if (!p.IsReadOnly)
            {
                node.Setter = new PropertyMethodNode()
                {
                    Name = "set",
                    Accessibility = p.SetMethod.DeclaredAccessibility.ToString().ToLower(),
                };
                if (node.Accessibility == node.Setter.Accessibility)
                {
                    node.Setter.Accessibility = string.Empty;
                }
                var setterContext = new ProxyMethodGeneratorContext(p.SetMethod, propertyGeneratorContext.ClassGeneratorContext);
                node.Setter.Body.Add("var ");
                node.Setter.Body.Add(setterContext.ReturnValueParameterName);
                node.Setter.Body.Add(" = value; ");
                foreach (var item in interceptors)
                {
                    node.Setter.Body.AddRange(item.BeforeMethod(setterContext));
                }
                node.Setter.Body.Add(propertyGeneratorContext.ClassGeneratorContext.ProxyFieldName);
                if (p.IsIndexer)
                {
                    node.Setter.Body.Add("[");
                    node.Setter.Body.Add(string.Join(",", p.Parameters.Select(i => i.Name)));
                    node.Setter.Body.Add("]");
                }
                else
                {
                    node.Setter.Body.Add(".");
                    node.Setter.Body.Add(p.Name);
                }
                node.Setter.Body.Add(" = ");
                node.Setter.Body.Add(setterContext.ReturnValueParameterName);
                node.Setter.Body.Add(";");
                foreach (var item in interceptors)
                {
                    node.Getter.Body.AddRange(item.AfterMethod(setterContext));
                }
            }
            return node;
        }
    }
}
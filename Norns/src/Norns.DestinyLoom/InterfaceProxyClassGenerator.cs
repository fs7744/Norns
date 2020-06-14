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

        public override void GenerateProxyClass(ProxyGeneratorContext context)
        {
            var @namespace = new NamespaceNode(context.Namespace);
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
            foreach (var member in context.Type.GetMembers().Union(context.Type.AllInterfaces.SelectMany(i => i.GetMembers())).Distinct())
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
            @namespace.Generate(context.Content);
        }
    }
}
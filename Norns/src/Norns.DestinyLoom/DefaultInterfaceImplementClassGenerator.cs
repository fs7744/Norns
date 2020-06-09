using Microsoft.CodeAnalysis;
using System.Linq;
using System.Text;

namespace Norns.DestinyLoom
{
    public class DefaultInterfaceImplementClassGenerator : AbstractProxyClassGenerator
    {
        public DefaultInterfaceImplementClassGenerator(IInterceptorGenerator[] interceptors) : base(interceptors)
        {
        }

        public override bool CanProxy(INamedTypeSymbol type)
        {
            return type.TypeKind == TypeKind.Interface;
        }

        public override string GenerateProxyClass(ProxyGeneratorContext context)
        {
            var @namespace = new NamespaceNode($"{context.Type.ContainingNamespace.ToDisplayString()}.Proxy{GuidHelper.NewGuidName()}");
            var @class = new ClassNode($"Proxy{context.Type.Name}{GuidHelper.NewGuidName()}");
            @class.CustomAttributes.Add("[Norns.Fate.Abstraction.DefaultInterfaceImplement(typeof(");
            @class.CustomAttributes.Add(context.Type.ToDisplayString());
            @class.CustomAttributes.Add("))]");
            @class.Accessibility = context.Type.DeclaredAccessibility.ToString().ToLower();
            @namespace.Classes.Add(@class);
            @class.Inherit.Types.Add(context.Type.ToDisplayString());
            foreach (var member in context.Type.GetMembers().Union(context.Type.AllInterfaces.SelectMany(i => i.GetMembers())).Distinct())
            {
                switch (member)
                {
                    case IMethodSymbol method when method.MethodKind != MethodKind.PropertyGet && method.MethodKind != MethodKind.PropertySet:
                        var methodGeneratorContext = new ProxyMethodGeneratorContext(method, context);
                        var m = GenerateProxyMethod(methodGeneratorContext);
                        if (m != null)
                        {
                            @class.Methods.Add(m);
                        }
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
            }
            return node;
        }

        public override MethodNode GenerateProxyMethod(ProxyMethodGeneratorContext context)
        {
            var method = context.Method;
            if (!method.IsAbstract) return null;
            var methodNode = new MethodNode()
            {
                Accessibility = method.DeclaredAccessibility.ToString().ToLower(),
                Return = method.ReturnType.ToDisplayString(),
                Name = method.Name,
            };
            foreach (var p in method.Parameters)
            {
                methodNode.Parameters.Add(new ParameterNode() { Type = p.Type.ToDisplayString(), Name = p.Name });
            }

            if (context.HasReturnValue || context.IsAsync)
            {
                var returnTypeStr = context.Method.ReturnType.ToDisplayString();
                if (context.IsAsyncValue)
                {
                    methodNode.Body.Add("return ");
                    methodNode.Body.Add(returnTypeStr.Replace("System.Threading.Tasks.Task", "System.Threading.Tasks.Task.FromResult"));
                    methodNode.Body.Add("(default);");
                }
                else if (context.IsAsync)
                {
                    methodNode.Body.Add("return System.Threading.Tasks.Task.CompletedTask;");
                }
                else
                {
                    methodNode.Body.Add("return default;");
                }
            }
            return methodNode;
        }
    }
}
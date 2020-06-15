using Microsoft.CodeAnalysis;
using Norns.DestinyLoom.Symbols;
using System.Linq;

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

        public override NamespaceSymbol GenerateProxyClass(ProxyGeneratorContext context)
        {
            var @namespace = Symbol.CreateNamespace(context.Namespace);
            var @class = Symbol.CreateClass($"Proxy{context.Type.Name}{GuidHelper.NewGuidName()}", context.Type.DeclaredAccessibility.ToString().ToLower());
            @class.CustomAttributes.AddLast($"[Norns.Fate.Abstraction.DefaultInterfaceImplement(typeof({context.Type.ToDisplayString()}))]".ToSymbol());
            @namespace.Members.AddLast(@class);
            @class.Inherits.AddLast(context.Type.ToDisplayString().ToSymbol());
            foreach (var member in context.Type.GetMembers().Union(context.Type.AllInterfaces.SelectMany(i => i.GetMembers())).Distinct())
            {
                switch (member)
                {
                    case IMethodSymbol method when method.MethodKind != MethodKind.PropertyGet && method.MethodKind != MethodKind.PropertySet:
                        var methodGeneratorContext = new ProxyMethodGeneratorContext(method, context);
                        var m = GenerateProxyMethod(methodGeneratorContext);
                        if (m != null)
                        {
                            @class.Members.AddLast(m);
                        }
                        break;

                    case IPropertySymbol property:
                        var propertyGeneratorContext = new ProxyPropertyGeneratorContext(property, context);
                        @class.Members.AddLast(GenerateProxyProperty(propertyGeneratorContext));
                        break;

                    default:
                        break;
                }
            }
            return @namespace;
        }

        public override PropertySymbol GenerateProxyProperty(ProxyPropertyGeneratorContext propertyGeneratorContext)
        {
            var p = propertyGeneratorContext.Property;
            var node = Symbol.CreateProperty(p.DeclaredAccessibility.ToString().ToLower(), p.Type.ToDisplayString(), p.Name);
            if (p.IsIndexer)
            {
                node.IsIndexer = true;
                foreach (var parameter in p.Parameters)
                {
                    node.Parameters.AddLast(Symbol.CreateParameter(parameter.Type.ToDisplayString(), parameter.Name));
                }
            }
            if (!p.IsWriteOnly)
            {
                node.CanRead = true;
                node.SetGetterAccessibility(p.GetMethod.DeclaredAccessibility.ToString().ToLower());
                if (p.IsIndexer)
                {
                    node.Getter.Body.AddLast("return default".ToSymbol());
                    node.Getter.Body.AddLast(Symbol.KeyOpenParen);
                    node.Getter.Body.AddLast(p.Type.ToDisplayString().ToSymbol());
                    node.Getter.Body.AddLast(Symbol.KeyCloseParen);
                    node.Getter.Body.AddLast(Symbol.KeySemicolon);
                }
            }
            if (!p.IsReadOnly)
            {
                node.CanWrite = true;
                node.SetSetterAccessibility(p.SetMethod.DeclaredAccessibility.ToString().ToLower());
                if (p.IsIndexer)
                {
                    node.Setter.Body.AddLast(Symbol.KeyBlank);
                }
            }
            return node;
        }

        public override MethodSymbol GenerateProxyMethod(ProxyMethodGeneratorContext context)
        {
            var method = context.Method;
            if (!method.IsAbstract) return null;
            var methodNode = Symbol.CreateMethod(method.DeclaredAccessibility.ToString().ToLower(), method.ReturnType.ToDisplayString(), method.Name);
            foreach (var p in method.Parameters)
            {
                methodNode.Parameters.AddLast(Symbol.CreateParameter(p.Type.ToDisplayString(), p.Name));
            }

            if (context.HasReturnValue || context.IsAsync)
            {
                var returnTypeStr = context.Method.ReturnType.ToDisplayString();
                if (context.IsAsyncValue)
                {
                    methodNode.Body.AddLast("return ".ToSymbol());
                    methodNode.Body.AddLast(returnTypeStr.Replace("System.Threading.Tasks.Task", "System.Threading.Tasks.Task.FromResult").ToSymbol());
                    methodNode.Body.AddLast("(default);".ToSymbol());
                }
                else if (context.IsAsync)
                {
                    methodNode.Body.AddLast("return System.Threading.Tasks.Task.CompletedTask;".ToSymbol());
                }
                else
                {
                    methodNode.Body.AddLast("return default;".ToSymbol());
                }
            }
            return methodNode;
        }
    }
}
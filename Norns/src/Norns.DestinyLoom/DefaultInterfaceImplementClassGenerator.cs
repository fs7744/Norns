﻿using Microsoft.CodeAnalysis;
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
            return true;
        }

        public override string GenerateProxyClass(ProxyGeneratorContext context)
        {
            var @namespace = new NamespaceNode($"{context.Type.ContainingNamespace.ToDisplayString()}.Proxy{GuidHelper.NewGuidName()}");
            var @class = new ClassNode($"Proxy{context.Type.Name}{GuidHelper.NewGuidName()}");
            @class.Accessibility = context.Type.DeclaredAccessibility.ToString().ToLower();
            @namespace.Classes.Add(@class);
            @class.Inherit.Types.Add(context.Type.ToDisplayString());
            foreach (var member in context.Type.GetMembers().Union(context.Type.AllInterfaces.SelectMany(i => i.GetMembers())).Distinct())
            {
                switch (member)
                {
                    case IMethodSymbol method:
                        var methodGeneratorContext = new ProxyMethodGeneratorContext(method, context);
                        var m = GenerateProxyMethod(methodGeneratorContext);
                        if (m != null)
                        {
                            @class.Methods.Add(m);
                        }
                        break;

                    default:
                        break;
                }
            }
            var sb = new StringBuilder();
            @namespace.Generate(sb);
            return sb.ToString();
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

            if (context.HasReturnValue)
            {
                var returnTypeStr = context.Method.ReturnType.ToDisplayString();
                if (returnTypeStr.StartsWith("System.Threading.Tasks.Task"))
                {
                    if (returnTypeStr.EndsWith(">"))
                    {
                        methodNode.Body.Add("return ");
                        methodNode.Body.Add(returnTypeStr.Replace("System.Threading.Tasks.Task", "System.Threading.Tasks.Task.FromResult"));
                        methodNode.Body.Add("(default);");
                    }
                    else
                    {
                        methodNode.Body.Add("return System.Threading.Tasks.Task.CompletedTask;");
                    }
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
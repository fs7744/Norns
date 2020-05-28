using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace Norns.DestinyLoom
{
    internal interface INodeGenerator
    {
        void Generate(StringBuilder sb);
    }

    internal class NamespaceNode : INodeGenerator
    {
        public string Name { get; }

        public List<ClassNode> Classes { get; } = new List<ClassNode>();

        public NamespaceNode(string name)
        {
            Name = name;
        }

        public void Generate(StringBuilder sb)
        {
            sb.Append("namespace ");
            sb.Append(Name);
            sb.Append(" { ");
            foreach (var classNode in Classes)
            {
                classNode.Generate(sb);
            }
            sb.Append(" } ");
        }
    }

    internal class InheritsNode : INodeGenerator
    {
        public List<string> Types { get; } = new List<string>();

        public void Generate(StringBuilder sb)
        {
            if (Types.Count == 0) return;
            sb.Append(" : ");
            for (int i = 0; i < Types.Count - 1; i++)
            {
                sb.Append(Types[i]);
                sb.Append(",");
            }
            sb.Append(Types[Types.Count - 1]);
        }
    }

    internal class ParameterNode : INodeGenerator
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public void Generate(StringBuilder sb)
        {
            sb.Append(Type);
            sb.Append(" ");
            sb.Append(Name);
        }
    }

        internal class MethodNode : INodeGenerator
    {
        public string Accessibility { get; set; }
        public string Return { get; set; }
        public string Name { get; set; }

        public List<ParameterNode> Parameters { get; } = new List<ParameterNode>();

        public List<string> Body { get; } = new List<string>();

        public void Generate(StringBuilder sb)
        {
            sb.Append(Accessibility);
            sb.Append(" ");
            sb.Append(Return);
            sb.Append(" ");
            sb.Append(Name);
            sb.Append("(");
            GenerateParameters(sb);
            sb.Append(")");
            sb.Append(" { ");
            foreach (var item in Body)
            {
                sb.Append(item);
            }
            sb.Append(" } ");
        }

        private void GenerateParameters(StringBuilder sb)
        {
            if (Parameters.Count == 0) return;
            Parameters[0].Generate(sb);
            for (int i = 1; i < Parameters.Count; i++)
            {
                sb.Append(",");
                Parameters[i].Generate(sb);
            }
        }
    }

    internal class ClassNode : INodeGenerator
    {
        public string Name { get; }

        public InheritsNode Inherit { get; } = new InheritsNode();

        public List<MethodNode> Methods { get; } = new List<MethodNode>();

        public ClassNode(string name)
        {
            Name = name;
        }

        public void Generate(StringBuilder sb)
        {
            sb.AppendLine("public class ");
            sb.Append(Name);
            Inherit.Generate(sb);
            sb.Append(" { ");
            foreach (var methodNode in Methods)
            {
                methodNode.Generate(sb);
            }
            sb.Append(" } ");
        }
    }

    internal abstract class AbstractProxyClassGenerator
    {
        protected IInterceptorGenerator[] interceptors;

        protected AbstractProxyClassGenerator(IInterceptorGenerator[] interceptors)
        {
            this.interceptors = interceptors;
        }

        internal (string fileName, string content) Generate(ProxyGeneratorContext context)
        {
            return ($"{context.Type.Name}Proxy{GuidHelper.NewGuidName()}.cs", GenerateProxyClass(context));
        }

        internal abstract string GenerateProxyClass(ProxyGeneratorContext context);

        internal MethodNode GenerateProxyMethod(ProxyMethodGeneratorContext context)
        {
            var method = context.Method;
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
                methodNode.Body.Add("var ");
                methodNode.Body.Add(context.ReturnValueParameterName);
                methodNode.Body.Add(" = default(");
                methodNode.Body.Add(methodNode.Return);
                methodNode.Body.Add(");");
            }

            if (context.HasReturnValue)
            {
                methodNode.Body.Add("return ");
                methodNode.Body.Add(context.ReturnValueParameterName);
                methodNode.Body.Add(";");
            }
            return methodNode;
        }
    }

    internal class InterfaceProxyClassGenerator : AbstractProxyClassGenerator
    {
        internal InterfaceProxyClassGenerator(IInterceptorGenerator[] interceptors) : base(interceptors)
        {
        }

        internal override string GenerateProxyClass(ProxyGeneratorContext context)
        {
            var @namespace = new NamespaceNode($"{context.Type.ContainingNamespace.ToDisplayString()}.Proxy{GuidHelper.NewGuidName()}");
            var @class = new ClassNode($"{context.Type.Name}Proxy{GuidHelper.NewGuidName()}");
            @namespace.Classes.Add(@class);
            @class.Inherit.Types.Add(context.Type.ToDisplayString());
            foreach (var member in context.Type.GetMembers())
            {
                switch (member)
                {
                    case IMethodSymbol method:
                        var methodGeneratorContext = new ProxyMethodGeneratorContext(method, context.SourceGeneratorContext);
                        @class.Methods.Add(GenerateProxyMethod(methodGeneratorContext));
                        break;

                    default:
                        break;
                }
            }
            var sb = new StringBuilder();
            @namespace.Generate(sb);
            return sb.ToString();
        }
    }

    internal class ClassProxyClassGenerator : AbstractProxyClassGenerator
    {
        internal ClassProxyClassGenerator(IInterceptorGenerator[] interceptors) : base(interceptors)
        {
        }

        internal override string GenerateProxyClass(ProxyGeneratorContext context)
        {
            return string.Empty;
        }
    }
}
using Microsoft.CodeAnalysis;
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

    public class ParameterNode : INodeGenerator
    {
        public string Type { get; set; }
        public string Name { get; set; }

        public void Generate(StringBuilder sb)
        {
            sb.Append(Type);
            sb.Append(" ");
            sb.Append(Name);
        }

        public void GeneratePassing(StringBuilder sb)
        {
            sb.Append(Name);
        }
    }

    public class MethodNode : INodeGenerator
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

        internal void GenerateParameters(List<string> body)
        {
            if (Parameters.Count == 0) return;
            body.Add(Parameters[0].Name);
            for (int i = 1; i < Parameters.Count; i++)
            {
                body.Add(",");
                body.Add(Parameters[i].Name);
            }
        }
    }

    internal class PropertyMethodNode : INodeGenerator
    {
        public string Name { get; set; }
        public string Accessibility { get; set; }

        public void Generate(StringBuilder sb)
        {
            sb.Append(Accessibility);
            sb.Append(" ");
            sb.Append(Name);
            sb.Append(";");
        }
    }

    internal class PropertyNode : INodeGenerator
    {
        public string Name { get; set; }
        public string Accessibility { get; set; }
        public string Type { get; set; }
        public PropertyMethodNode Getter { get; set; }
        public PropertyMethodNode Setter { get; set; }

        public void Generate(StringBuilder sb)
        {
            sb.Append(Accessibility);
            sb.Append(" ");
            sb.Append(Type);
            sb.Append(" ");
            sb.Append(Name);
            sb.Append(" { ");
            Getter?.Generate(sb);
            sb.Append(" ");
            Setter?.Generate(sb);
            sb.Append(" } ");
        }
    }

    internal class ClassNode : INodeGenerator
    {
        public string Name { get; }

        public InheritsNode Inherit { get; } = new InheritsNode();

        public List<MethodNode> Methods { get; } = new List<MethodNode>();

        public List<CtorNode> Ctors { get; } = new List<CtorNode>();
        public string Accessibility { get; set; }
        public List<PropertyNode> Properties { get; set; } = new List<PropertyNode>();

        public ClassNode(string name)
        {
            Name = name;
        }

        public void Generate(StringBuilder sb)
        {
            sb.Append(Accessibility);
            sb.Append(" class ");
            sb.Append(Name);
            Inherit.Generate(sb);
            sb.Append(" { ");
            foreach (var p in Properties)
            {
                p.Generate(sb);
            }
            foreach (var methodNode in Methods)
            {
                methodNode.Generate(sb);
            }
            sb.Append(" } ");
        }
    }

    internal class CtorNode : INodeGenerator
    {
        public void Generate(StringBuilder sb)
        {
        }
    }

    public abstract class AbstractProxyClassGenerator
    {
        protected IInterceptorGenerator[] interceptors;

        protected AbstractProxyClassGenerator(IInterceptorGenerator[] interceptors)
        {
            this.interceptors = interceptors;
        }

        public (string fileName, string content) Generate(ProxyGeneratorContext context)
        {
            return ($"Proxy{context.Type.Name}{GuidHelper.NewGuidName()}.cs", GenerateProxyClass(context));
        }

        public abstract string GenerateProxyClass(ProxyGeneratorContext context);

        public abstract bool CanProxy(INamedTypeSymbol type);

        public virtual MethodNode GenerateProxyMethod(ProxyMethodGeneratorContext context)
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

            foreach (var item in interceptors)
            {
                methodNode.Body.AddRange(item.BeforeMethod(context));
            }

            if (!method.IsAbstract)
            {
                if (context.HasReturnValue)
                {
                    methodNode.Body.Add(context.ReturnValueParameterName);
                    methodNode.Body.Add(" = ");
                }
                methodNode.Body.Add("base.");
                methodNode.Body.Add(method.Name);
                methodNode.Body.Add("(");
                methodNode.GenerateParameters(methodNode.Body);
                methodNode.Body.Add(");");
            }

            foreach (var item in interceptors)
            {
                methodNode.Body.AddRange(item.AfterMethod(context));
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
            @namespace.Classes.Add(@class);
            @class.Inherit.Types.Add(context.Type.ToDisplayString());
            foreach (var member in context.Type.GetMembers())
            {
                switch (member)
                {
                    case IMethodSymbol method:
                        var methodGeneratorContext = new ProxyMethodGeneratorContext(method, context);
                        var m = GenerateProxyMethod(methodGeneratorContext);
                        @class.Methods.Add(m);
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

    public class ClassProxyClassGenerator : AbstractProxyClassGenerator
    {
        public ClassProxyClassGenerator(IInterceptorGenerator[] interceptors) : base(interceptors)
        {
        }

        public override bool CanProxy(INamedTypeSymbol type)
        {
            return @type.TypeKind == TypeKind.Class;
        }

        public override string GenerateProxyClass(ProxyGeneratorContext context)
        {
            return string.Empty;
        }
    }
}
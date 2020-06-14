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
        public List<string> Symbols { get; } = new List<string>();
        public List<string> Constraints { get; } = new List<string>();

        public List<ParameterNode> Parameters { get; } = new List<ParameterNode>();

        public List<string> Body { get; } = new List<string>();

        public void Generate(StringBuilder sb)
        {
            sb.Append(Accessibility);
            sb.Append(" ");
            foreach (var item in Symbols)
            {
                sb.Append(item);
                sb.Append(" ");
            }
            sb.Append(" ");
            sb.Append(Return);
            sb.Append(" ");
            sb.Append(Name);
            sb.Append("(");
            GenerateParameters(sb);
            sb.Append(")");
            foreach (var item in Constraints)
            {
                sb.Append(item);
            }
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

    public class PropertyMethodNode : INodeGenerator
    {
        public string Name { get; set; }
        public string Accessibility { get; set; }

        public List<string> Body { get; } = new List<string>();

        public void Generate(StringBuilder sb)
        {
            sb.Append(Accessibility);
            sb.Append(" ");
            sb.Append(Name);
            if (Body.Count > 0)
            {
                sb.Append(" { ");
                foreach (var item in Body)
                {
                    sb.Append(item);
                }
                sb.Append(" } ");
            }
            else
            {
                sb.Append(";");
            }
        }
    }

    public class PropertyNode : INodeGenerator
    {
        public string Name { get; set; }
        public string Accessibility { get; set; }
        public string Type { get; set; }
        public PropertyMethodNode Getter { get; set; }
        public PropertyMethodNode Setter { get; set; }
        public List<string> Symbols { get; } = new List<string>();

        public void Generate(StringBuilder sb)
        {
            sb.Append(Accessibility);
            sb.Append(" ");
            foreach (var item in Symbols)
            {
                sb.Append(item);
                sb.Append(" ");
            }
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

    internal class FieldNode : INodeGenerator
    {
        public string Name { get; set; }
        public string Accessibility { get; set; }
        public string Type { get; set; }

        public void Generate(StringBuilder sb)
        {
            sb.Append(Accessibility);
            sb.Append(" ");
            sb.Append(Type);
            sb.Append(" ");
            sb.Append(Name);
            sb.Append(";");
        }
    }

    internal class ClassNode : INodeGenerator
    {
        public string Name { get; }

        public InheritsNode Inherit { get; } = new InheritsNode();

        public List<MethodNode> Methods { get; } = new List<MethodNode>();
        public List<FieldNode> Fields { get; } = new List<FieldNode>();

        public List<CtorNode> Ctors { get; } = new List<CtorNode>();
        public string Accessibility { get; set; }
        public List<PropertyNode> Properties { get; set; } = new List<PropertyNode>();
        public List<string> CustomAttributes { get; } = new List<string>();

        public ClassNode(string name)
        {
            Name = name;
        }

        public void Generate(StringBuilder sb)
        {
            foreach (var a in CustomAttributes)
            {
                sb.Append(a);
            }
            sb.Append(" ");
            sb.Append(Accessibility);
            sb.Append(" class ");
            sb.Append(Name);
            Inherit.Generate(sb);
            sb.Append(" { ");
            foreach (var f in Fields)
            {
                f.Generate(sb);
            }
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
}
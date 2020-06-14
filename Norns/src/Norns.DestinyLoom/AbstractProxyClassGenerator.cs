using Microsoft.CodeAnalysis;
using System.Linq;

namespace Norns.DestinyLoom
{
    public abstract class AbstractProxyClassGenerator
    {
        protected IInterceptorGenerator[] interceptors;

        protected AbstractProxyClassGenerator(IInterceptorGenerator[] interceptors)
        {
            this.interceptors = interceptors;
        }

        public void Generate(ProxyGeneratorContext context)
        {
            GenerateProxyClass(context);
        }

        public abstract void GenerateProxyClass(ProxyGeneratorContext context);

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
            if (context.IsAsync)
            {
                methodNode.Symbols.Add("async");
            }
            foreach (var p in method.Parameters)
            {
                methodNode.Parameters.Add(new ParameterNode() { Type = p.Type.ToDisplayString(), Name = p.Name });
            }
            if (context.HasReturnValue)
            {
                //methodNode.Body.Add("var ");
                //methodNode.Body.Add(context.ReturnValueParameterName);
                //var returnTypeStr = context.Method.ReturnType.ToDisplayString();

                if (context.IsAsyncValue)
                {
                    methodNode.Body.Add("var ");
                    methodNode.Body.Add(context.ReturnValueParameterName);
                    methodNode.Body.Add(" = ");
                    //methodNode.Body.Add(returnTypeStr.Replace("System.Threading.Tasks.Task", "System.Threading.Tasks.Task.FromResult"));
                    methodNode.Body.Add("default(");
                    methodNode.Body.Add(context.AsyncValueType);
                    methodNode.Body.Add(");");
                }
                //else if (context.IsAsync)
                //{
                //    methodNode.Body.Add(" = System.Threading.Tasks.Task.CompletedTask;");
                //}
                else
                {
                    methodNode.Body.Add("var ");
                    methodNode.Body.Add(context.ReturnValueParameterName);
                    methodNode.Body.Add(" = default(");
                    methodNode.Body.Add(methodNode.Return);
                    methodNode.Body.Add(");");
                }
            }

            foreach (var item in interceptors)
            {
                methodNode.Body.AddRange(item.BeforeMethod(context));
            }
            if (method.ContainingType.TypeKind == TypeKind.Interface || !method.IsAbstract)
            {
                if (context.HasReturnValue)
                {
                    methodNode.Body.Add(context.ReturnValueParameterName);
                    methodNode.Body.Add(" = ");
                }
                if (context.IsAsync)
                {
                    methodNode.Body.Add("await ");
                }
                methodNode.Body.Add(context.ClassGeneratorContext.ProxyFieldName);
                methodNode.Body.Add(".");
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

        public virtual PropertyNode GenerateProxyProperty(ProxyPropertyGeneratorContext propertyGeneratorContext)
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
using Microsoft.CodeAnalysis;

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
}
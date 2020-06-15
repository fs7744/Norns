using Microsoft.CodeAnalysis;
using Norns.DestinyLoom.Symbols;
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

        public NamespaceSymbol Generate(ProxyGeneratorContext context)
        {
            return GenerateProxyClass(context);
        }

        public abstract bool CanProxy(INamedTypeSymbol type);

        public virtual MethodSymbol GenerateProxyMethod(ProxyMethodGeneratorContext context)
        {
            var method = context.Method;
            var methodNode = Symbol.CreateMethod(method.DeclaredAccessibility.ToString().ToLower(), method.ReturnType.ToDisplayString(), method.Name);
            if (context.IsAsync)
            {
                methodNode.Symbols.AddLast(Symbol.KeyAsync);
            }
            foreach (var p in method.Parameters)
            {
                methodNode.Parameters.AddLast(Symbol.CreateParameter(p.Type.ToDisplayString(), p.Name));
            }
            if (context.HasReturnValue)
            {
                if (context.IsAsyncValue)
                {
                    methodNode.Body.AddLast("var ".ToSymbol());
                    methodNode.Body.AddLast(context.ReturnValueParameterName.ToSymbol());
                    methodNode.Body.AddLast(" = ".ToSymbol());
                    methodNode.Body.AddLast("default(".ToSymbol());
                    methodNode.Body.AddLast(context.AsyncValueType.ToSymbol());
                    methodNode.Body.AddLast(");".ToSymbol());
                }
                else
                {
                    methodNode.Body.AddLast("var ".ToSymbol());
                    methodNode.Body.AddLast(context.ReturnValueParameterName.ToSymbol());
                    methodNode.Body.AddLast(" = default(".ToSymbol());
                    methodNode.Body.AddLast(methodNode.ReturnType.ToSymbol());
                    methodNode.Body.AddLast(");".ToSymbol());
                }
            }

            methodNode.Body.AddLast(interceptors.GenerateSymbolBeforeMethod(context));
            if (method.ContainingType.TypeKind == TypeKind.Interface || !method.IsAbstract)
            {
                if (context.HasReturnValue)
                {
                    methodNode.Body.AddLast(context.ReturnValueParameterName.ToSymbol());
                    methodNode.Body.AddLast(" = ".ToSymbol());
                }
                if (context.IsAsync)
                {
                    methodNode.Body.AddLast("await ".ToSymbol());
                }
                methodNode.Body.AddLast(context.ClassGeneratorContext.ProxyFieldName.ToSymbol());
                methodNode.Body.AddLast(".".ToSymbol());
                methodNode.Body.AddLast(method.Name.ToSymbol());
                methodNode.Body.AddLast(methodNode.CallParameters);
                methodNode.Body.AddLast(Symbol.KeySemicolon);
            }
            methodNode.Body.AddLast(interceptors.GenerateSymbolAfterMethod(context));

            if (context.HasReturnValue)
            {
                methodNode.Body.AddLast("return ".ToSymbol());
                methodNode.Body.AddLast(context.ReturnValueParameterName.ToSymbol());
                methodNode.Body.AddLast(";".ToSymbol());
            }
            return methodNode;
        }

        public virtual PropertySymbol GenerateProxyProperty(ProxyPropertyGeneratorContext propertyGeneratorContext)
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
                var getterContext = new ProxyMethodGeneratorContext(p.GetMethod, propertyGeneratorContext.ClassGeneratorContext);
                node.Getter.Body.AddLast("var ".ToSymbol());
                node.Getter.Body.AddLast(getterContext.ReturnValueParameterName.ToSymbol());
                node.Getter.Body.AddLast(" = default".ToSymbol());
                node.Getter.Body.AddLast(Symbol.KeyOpenParen);
                node.Getter.Body.AddLast(p.Type.ToDisplayString().ToSymbol());
                node.Getter.Body.AddLast(Symbol.KeyCloseParen);
                node.Getter.Body.AddLast(Symbol.KeySemicolon);
                node.Getter.Body.AddLast(interceptors.GenerateSymbolBeforeMethod(getterContext));
                node.Getter.Body.AddLast(getterContext.ReturnValueParameterName.ToSymbol());
                node.Getter.Body.AddLast(" = ".ToSymbol());
                node.Getter.Body.AddLast(propertyGeneratorContext.ClassGeneratorContext.ProxyFieldName.ToSymbol());
                if (p.IsIndexer)
                {
                    node.Getter.Body.AddLast(node.CallIndexerParameters);
                }
                else
                {
                    node.Getter.Body.AddLast(".".ToSymbol());
                    node.Getter.Body.AddLast(p.Name.ToSymbol());
                }
                node.Getter.Body.AddLast(Symbol.KeySemicolon);
                node.Getter.Body.AddLast(interceptors.GenerateSymbolAfterMethod(getterContext));
                node.Getter.Body.AddLast("return ".ToSymbol());
                node.Getter.Body.AddLast(getterContext.ReturnValueParameterName.ToSymbol());
                node.Getter.Body.AddLast(Symbol.KeySemicolon);
            }
            if (!p.IsReadOnly)
            {
                node.CanWrite = true;
                node.SetSetterAccessibility(p.SetMethod.DeclaredAccessibility.ToString().ToLower());
                var setterContext = new ProxyMethodGeneratorContext(p.SetMethod, propertyGeneratorContext.ClassGeneratorContext);
                node.Setter.Body.AddLast("var ".ToSymbol());
                node.Setter.Body.AddLast(setterContext.ReturnValueParameterName.ToSymbol());
                node.Setter.Body.AddLast(" = value; ".ToSymbol());
                node.Setter.Body.AddLast(interceptors.GenerateSymbolBeforeMethod(setterContext));
                node.Setter.Body.AddLast(propertyGeneratorContext.ClassGeneratorContext.ProxyFieldName.ToSymbol());
                if (p.IsIndexer)
                {
                    node.Setter.Body.AddLast(node.CallIndexerParameters);
                }
                else
                {
                    node.Setter.Body.AddLast(".".ToSymbol());
                    node.Setter.Body.AddLast(p.Name.ToSymbol());
                }
                node.Setter.Body.AddLast(" = ".ToSymbol());
                node.Setter.Body.AddLast(setterContext.ReturnValueParameterName.ToSymbol());
                node.Setter.Body.AddLast(Symbol.KeySemicolon);
                node.Setter.Body.AddLast(interceptors.GenerateSymbolAfterMethod(setterContext));
            }
            return node;
        }

        public virtual NamespaceSymbol GenerateProxyClass(ProxyGeneratorContext context)
        {
            var @namespace = Symbol.CreateNamespace(context.Namespace);
            var @class = Symbol.CreateClass($"Proxy{context.Type.Name}{GuidHelper.NewGuidName()}", context.Type.DeclaredAccessibility.ToString().ToLower());
            @class.CustomAttributes.AddLast($"[Norns.Fate.Abstraction.Proxy(typeof({context.Type.ToDisplayString()}))]".ToSymbol());
            @namespace.Members.AddLast(@class);
            @class.Members.AddLast(Symbol.CreateField("public", context.Type.ToDisplayString(), context.ProxyFieldName));
            var setProxyNode = Symbol.CreateMethod("public", "void", "SetProxy");
            setProxyNode.Parameters.AddLast(Symbol.CreateParameter("object", "instance"));
            setProxyNode.Parameters.AddLast(Symbol.CreateParameter("System.IServiceProvider", "serviceProvider"));
            setProxyNode.Body.AddLast(context.ProxyFieldName.ToSymbol());
            setProxyNode.Body.AddLast(" = instance as ".ToSymbol());
            setProxyNode.Body.AddLast(context.Type.ToDisplayString().ToSymbol());
            setProxyNode.Body.AddLast(";".ToSymbol());
            @class.Members.AddLast(setProxyNode);
            @class.Inherits.AddLast(context.Type.ToDisplayString().ToSymbol());
            @class.Inherits.AddLast("Norns.Fate.Abstraction.IInterceptProxy".ToSymbol());

            foreach (var member in context.Type.GetMembers().Where(i => i.IsAbstract || i.IsVirtual || i.IsOverride || (i is IMethodSymbol m) && m.MethodKind == MethodKind.Constructor)
                .Union(context.Type.AllInterfaces.SelectMany(i => i.GetMembers())).Distinct())
            {
                switch (member)
                {
                    case IMethodSymbol method when method.MethodKind == MethodKind.Constructor:
                        {
                            var methodGeneratorContext = new ProxyMethodGeneratorContext(method, context)
                            {
                                ClassName = @class.Name
                            };
                            var m = GenerateProxyConstructor(methodGeneratorContext);
                            @class.Members.AddLast(m);
                        }
                        break;

                    case IMethodSymbol method when method.MethodKind != MethodKind.PropertyGet && method.MethodKind != MethodKind.PropertySet:
                        {
                            var methodGeneratorContext = new ProxyMethodGeneratorContext(method, context);
                            var m = GenerateProxyMethod(methodGeneratorContext);
                            @class.Members.AddLast(m);
                        }
                        break;

                    case IPropertySymbol property:
                        var propertyGeneratorContext = new ProxyPropertyGeneratorContext(property, context);
                        var p = GenerateProxyProperty(propertyGeneratorContext);
                        @class.Members.AddLast(p);
                        break;

                    default:
                        break;
                }
            }
            return @namespace;
        }

        public virtual MethodSymbol GenerateProxyConstructor(ProxyMethodGeneratorContext context)
        {
            var method = context.Method;
            var methodNode = Symbol.CreateMethod(method.DeclaredAccessibility.ToString().ToLower(), string.Empty, context.ClassName);
            foreach (var parameter in method.Parameters)
            {
                methodNode.Parameters.AddLast(Symbol.CreateParameter(parameter.Type.ToDisplayString(), parameter.Name));
            }
            methodNode.Constraints.AddLast(" : base".ToSymbol());
            methodNode.Constraints.AddLast(methodNode.CallParameters);
            methodNode.Body.AddLast(interceptors.GenerateSymbolBeforeMethod(context));
            methodNode.Body.AddLast(interceptors.GenerateSymbolAfterMethod(context));
            return methodNode;
        }
    }
}
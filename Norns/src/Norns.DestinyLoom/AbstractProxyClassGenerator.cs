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
            var methodNode = Symbol.CreateMethod(context.Accessibility, method.ReturnType.ToDisplayString(), method.Name);
            if (context.IsAsync)
            {
                methodNode.Symbols.Add(Symbol.KeyAsync);
            }
            foreach (var p in method.Parameters)
            {
                methodNode.Parameters.Add(Symbol.CreateParameter(p.Type.ToDisplayString(), p.Name));
            }
            if (context.HasReturnValue)
            {
                methodNode.AddBody("var ", context.ReturnValueParameterName, " = default(", context.IsAsyncValue ? context.AsyncValueType : methodNode.ReturnType, ");");
            }

            methodNode.AddBody(interceptors.GenerateSymbolBeforeMethod(context));
            if (method.ContainingType.TypeKind == TypeKind.Interface || !method.IsAbstract)
            {
                if (context.HasReturnValue)
                {
                    methodNode.AddBody(context.ReturnValueParameterName, " = ");
                }
                methodNode.AddBody(context.IsAsync ? "await " : string.Empty, context.ClassGeneratorContext.ProxyFieldName, ".", method.Name);
                methodNode.AddBody(methodNode.CallParameters, Symbol.KeySemicolon);
            }
            methodNode.AddBody(interceptors.GenerateSymbolAfterMethod(context));

            if (context.HasReturnValue)
            {
                methodNode.AddBody("return ", context.ReturnValueParameterName, ";");
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
                node.Getter.AddBody("var ", getterContext.ReturnValueParameterName, " = default(", p.Type.ToDisplayString(), ");");
                node.Getter.AddBody(interceptors.GenerateSymbolBeforeMethod(getterContext));
                node.Getter.AddBody(getterContext.ReturnValueParameterName, " = ", propertyGeneratorContext.ClassGeneratorContext.ProxyFieldName);
                node.AddCallGetter();
                node.Getter.AddBody(Symbol.KeySemicolon, interceptors.GenerateSymbolAfterMethod(getterContext));
                node.Getter.AddBody("return ", getterContext.ReturnValueParameterName, ";");
            }
            if (!p.IsReadOnly)
            {
                node.CanWrite = true;
                node.SetSetterAccessibility(p.SetMethod.DeclaredAccessibility.ToString().ToLower());
                var setterContext = new ProxyMethodGeneratorContext(p.SetMethod, propertyGeneratorContext.ClassGeneratorContext);
                node.Setter.AddBody("var ", setterContext.ReturnValueParameterName, " = value; ");
                node.Setter.AddBody(interceptors.GenerateSymbolBeforeMethod(setterContext));
                node.Setter.Body.AddLast(propertyGeneratorContext.ClassGeneratorContext.ProxyFieldName.ToSymbol());
                node.AddCallSetter();
                node.Setter.AddBody(" = ", setterContext.ReturnValueParameterName);
                node.Setter.AddBody(Symbol.KeySemicolon, interceptors.GenerateSymbolAfterMethod(setterContext));
            }
            return node;
        }

        public void AddProxyInfo(ClassSymbol @class, ProxyGeneratorContext context)
        {
            @class.Members.Add(Symbol.CreateField("public", context.Type.ToDisplayString(), context.ProxyFieldName));
            var setProxyNode = Symbol.CreateMethod("public", "void", "SetProxy");
            setProxyNode.Parameters.Add(Symbol.CreateParameter("object", "instance"));
            setProxyNode.Parameters.Add(Symbol.CreateParameter("System.IServiceProvider", "serviceProvider"));
            setProxyNode.AddBody(context.ProxyFieldName, " = instance as ", context.Type.ToDisplayString(), ";");
            @class.Members.Add(setProxyNode);
            @class.Inherits.Add("Norns.Fate.Abstraction.IInterceptProxy");
            foreach (var f in context.DIFields.Values)
            {
                @class.Members.Add(f);
                setProxyNode.AddBody(f.Name, " = serviceProvider.GetService(typeof(", f.Type, ")) as ", f.Type, ";");
            }
        }

        public virtual NamespaceSymbol GenerateProxyClass(ProxyGeneratorContext context)
        {
            var @namespace = Symbol.CreateNamespace(context.Namespace);
            var @class = Symbol.CreateClass($"Proxy{context.Type.Name}{GuidHelper.NewGuidName()}", context.Accessibility);
            @class.CustomAttributes.Add($"[Norns.Fate.Abstraction.Proxy(typeof({context.Type.ToDisplayString()}))]");
            @class.Inherits.Add(context.Type.ToDisplayString());
            @namespace.Members.Add(@class);

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
                            @class.Members.Add(m);
                        }
                        break;

                    case IMethodSymbol method when method.MethodKind != MethodKind.PropertyGet && method.MethodKind != MethodKind.PropertySet:
                        {
                            var methodGeneratorContext = new ProxyMethodGeneratorContext(method, context);
                            var m = GenerateProxyMethod(methodGeneratorContext);
                            @class.Members.Add(m);
                        }
                        break;

                    case IPropertySymbol property:
                        var propertyGeneratorContext = new ProxyPropertyGeneratorContext(property, context);
                        var p = GenerateProxyProperty(propertyGeneratorContext);
                        @class.Members.Add(p);
                        break;

                    default:
                        break;
                }
            }
            AddProxyInfo(@class, context);
            @namespace.Usings.Add(context.Usings.Values.ToArray<IGenerateSymbol>());
            return @namespace;
        }

        public virtual MethodSymbol GenerateProxyConstructor(ProxyMethodGeneratorContext context)
        {
            var method = context.Method;
            var methodNode = Symbol.CreateMethod(context.Accessibility, string.Empty, context.ClassName);
            foreach (var parameter in method.Parameters)
            {
                methodNode.Parameters.Add(Symbol.CreateParameter(parameter.Type.ToDisplayString(), parameter.Name));
            }
            methodNode.Constraints.Add(" : base".ToSymbol(), methodNode.CallParameters, interceptors.GenerateSymbolBeforeMethod(context), interceptors.GenerateSymbolAfterMethod(context));
            return methodNode;
        }
    }
}
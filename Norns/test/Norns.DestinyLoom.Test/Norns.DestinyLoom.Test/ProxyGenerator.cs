using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Norns.DestinyLoom.Test
{
    [Generator]
    public class ProxyGenerator : AbstractProxyGenerator
    {
        public override IEnumerable<IInterceptorGenerator> FindInterceptorGenerators()
        {
            yield return new ConsoleCall();
        }

        public override bool CanProxy(INamedTypeSymbol @type)
        {
            //return false;
            //return @type.ToDisplayString().StartsWith("Norns.Benchmark.IC");
           return @type.ToDisplayString().StartsWith("Norns");
        }

        public override IEnumerable<AbstractProxyClassGenerator> FindProxyClassGenerators(IInterceptorGenerator[] interceptors)
        {
            yield return new DefaultInterfaceImplementClassGenerator(interceptors);
            yield return new InterfaceProxyClassGenerator(interceptors);
            //yield return new ClassProxyClassGenerator(interceptors);
        }
    }

    public class ConsoleCall : IInterceptorGenerator
    {
        public IEnumerable<string> BeforeMethod(ProxyMethodGeneratorContext context)
        {
            if (!context.Method.Parameters.IsEmpty)
            {
                yield return "System.Console.WriteLine($\"Call Method";
                yield return context.Method.Name;
                yield return " ";
                yield return context.Method.Parameters[0].Type.ToDisplayString();
                yield return " ";
                yield return context.Method.Parameters[0].Name;
                yield return " = {";
                yield return context.Method.Parameters[0].Name;
                yield return "}";
                foreach (var item in context.Method.Parameters.Skip(1))
                {
                    yield return ", ";
                    yield return item.ToDisplayString();
                    yield return " ";
                    yield return item.Name;
                    yield return " = {";
                    yield return item.Name;
                    yield return "}";
                }
                yield return "\");";
            }
        }

        public IEnumerable<string> AfterMethod(ProxyMethodGeneratorContext context)
        {
            if (context.HasReturnValue)
            {
                yield return "System.Console.WriteLine($\"";
                yield return $"return {{{context.ReturnValueParameterName}}}";
                yield return "\");";
            }
        }
    }
}
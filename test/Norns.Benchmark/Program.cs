using Microsoft.Extensions.DependencyInjection;
using Norns.Destiny.AOP;
using Norns.Destiny.Attributes;
using Norns.Destiny.Utils;
using System;
using Norns.Destiny.RuntimeSymbol;
using System.Linq;

namespace Norns.Benchmark
{
    [Charon]
    public interface IC
    {
        int AddOne(int v);

        public int AddOne2(int v)
        {
            return v + 1;
        }
    }

    public class C : IC
    {
        public int AddOne(int v)
        {
            return v;
        }

        public int AddOne2(int v)
        {
            return v + 3;
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var type = typeof(C).GetSymbolInfo();
            var dd = type.Members;
            var d = type.Members.Union(type.Interfaces.SelectMany(i => i.Members)).Distinct().ToArray();
            var p = new ServiceCollection()
                .AddTransient<IC, C>()
              //.AddDestinyInterface<IC>(ServiceLifetime.Scoped)
              .BuildVerthandiAopServiceProvider(null, new IInterceptorGenerator[] { new ConsoleCallMethodGenerator() })
              //.BuildAopServiceProvider(AppDomain.CurrentDomain.GetAssemblies())
              //.BuildServiceProvider()
              .GetRequiredService<IC>();
            DestinyExtensions.CleanCache();
            GC.Collect();
            var result = p.AddOne(99);
            Console.WriteLine($"p.AddOne(99) 's result is {result}.");
            Console.WriteLine();
            result = p.AddOne2(1);
            Console.WriteLine($"p.AddOne2(1) 's result is {result}.");

            Console.ReadKey();
        }
    }
}
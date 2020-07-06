using Microsoft.Extensions.DependencyInjection;
using Norns.Destiny.AOP;
using Norns.Destiny.Attributes;
using Norns.Destiny.Utils;
using System;

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
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            var p = new ServiceCollection()
                .AddTransient<IC, C>()
              //.AddDestinyInterface<IC>(ServiceLifetime.Scoped)
              .BuildJitAopServiceProvider(null, new IInterceptorGenerator[] { new ConsoleCallMethodGenerator() }, AppDomain.CurrentDomain.GetAssemblies())
              //.BuildAopServiceProvider(AppDomain.CurrentDomain.GetAssemblies())
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
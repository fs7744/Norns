using Microsoft.Extensions.DependencyInjection;
using Norns.Destiny.AOP;
using Norns.Destiny.Attributes;
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

    internal class Program
    {
        private static void Main(string[] args)
        {
            var p = new ServiceCollection()
              .AddDestinyInterface<IC>(ServiceLifetime.Scoped)
              .BuildJitAopServiceProvider(null, new IInterceptorGenerator[] { new ConsoleCallMethodGenerator() }, AppDomain.CurrentDomain.GetAssemblies())
              //.BuildAopServiceProvider(AppDomain.CurrentDomain.GetAssemblies())
              .GetRequiredService<IC>();

            p.AddOne2(1);
        }
    }
}
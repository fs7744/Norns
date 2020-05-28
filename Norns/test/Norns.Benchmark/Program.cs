using BenchmarkDotNet.Running;
using Norns.Benchmark.Fate;
using System;

namespace Norns.Benchmark
{
    static class Program
    {
        static void Main(string[] args)
        {
            //new InterceptorBenchmarkTest().Test().Wait();
            BenchmarkRunner.Run<InterceptorBenchmarkTest>();
        }
    }

    public interface IC
    {
        int AddOne(int v);
    }

    public class C : IC
    {
        public int AddOne(int v)
        {
            return v + 1;
        }
    }

    //class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        var p = new ServiceCollection()
    //            .AddSingleton<IC, C>()
    //            .BuildAopServiceProvider()
    //            .GetRequiredService<IC>();

    //        p.AddOne(1);
    //    }
    //}
}

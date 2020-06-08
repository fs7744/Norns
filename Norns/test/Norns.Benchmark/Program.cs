using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using Norns.Benchmark.Fate;
using System;

namespace Norns.Benchmark
{
    //static class Program
    //{
    //    static void Main(string[] args)
    //    {
    //        //new InterceptorBenchmarkTest().Test().Wait();
    //        BenchmarkRunner.Run<InterceptorBenchmarkTest>();
    //    }
    //}

    public interface IC
    {
        int AddOne(int v);


        public int AddOne2(int v)
        {
            return v + 1;
        }
    }

    public class D : IC
    {
        public virtual int AddOne(int v)
        {
            return default;
        }
    }

    public class DsProxy : IC
    {
        IC d = new D();

        public int AddOne(int v)
        {
            return d.AddOne(v);
        }

        public int AddOne2(int v)
        {
            return d.AddOne2(v);
        }
    }

    public class C : IC
    {
        public int AddOne(int v)
        {
            return v + 1;
        }

        //int IC.AddOne2(int v)
        //{
        //     return 2;
        //}
    }

    class Program
    {
        static void Main(string[] args)
        {
            var p = new ServiceCollection()
                .AddSingleton<IC, DsProxy>()
                .BuildAopServiceProvider()
                .GetRequiredService<IC>();

            p.AddOne2(1);
        }
    }
}

using BenchmarkDotNet.Running;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Norns.ProxyGenerators.Test
{
    public interface ICSD
    {
        string this[int a, int b] { get; set; }

        Task<ValueTuple<int, int>> AddOne(int v);
    }

    public interface ICD : ICSD
    {
        Task<ValueTuple<int, int>> AddOne2(int v);
    }
}

namespace ProxyGenerators.Test.Proxyc713170faed04810ab2c37adc1214595
{
    public class ProxyIC678a642569b74ae39ddc3acf2bf04161 : Norns.ProxyGenerators.Test.ICD, Norns.ProxyGenerators.Test.ICSD
    {
        public string this[int a, int b] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public Task<ValueTuple<int, int>> AddOne(int v)
        {
            return System.Threading.Tasks.Task.FromResult<ValueTuple<int, int>>(default);
        }


        public Task<ValueTuple<int, int>> AddOne2(int v)
        {
            return System.Threading.Tasks.Task.FromResult<ValueTuple<int, int>>(default);
        }
    }
}

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

        //int AddOne2(int v);
        public int AddOne2(int v)
        {
            return v + 1;
        }
    }

    //public class D : IC
    //{
    //    public virtual int AddOne(int v)
    //    {
    //        return default;
    //    }

    //    public virtual int AddOne2(int v)
    //    {
    //        return default;
    //    }
    //}

    //public class DsProxy : IC
    //{
    //    IC d = new D();

    //    public int AddOne(int v)
    //    {
    //        return d.AddOne(v);
    //    }

    //    public int AddOne2(int v)
    //    {
    //        return d.AddOne2(v);
    //    }
    //}

    //public class C : IC
    //{
    //    public int AddOne(int v)
    //    {
    //        return v + 1;
    //    }

    //    public int AddOne2(int v)
    //    {
    //        return 2;
    //    }
    //}

    class Program
    {
        static void Main(string[] args)
        {
            var p = new ServiceCollection()
                //.AddSingleton<IC, DsProxy>()
                .AddFateInterface<IC>(ServiceLifetime.Scoped)
                .BuildAopServiceProvider()
                .GetRequiredService<IC>();

            p.AddOne2(1);
        }
    }
}

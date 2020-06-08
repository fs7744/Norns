using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Threading.Tasks;

namespace Benchmark.Fate
{
    public delegate Task InterceptAsync(FateContext context);

    public delegate void Intercept(FateContext context);

    public interface IInterceptor
    {
        void Invoke(FateContext context, Intercept next);

        Task InvokeAsync(FateContext context, InterceptAsync next);
    }

    public interface IInterceptProxy
    {
        public IServiceProvider Provider { get; set; }
    }

    public interface ITest
    {
        Task<int> AddOneAsync(int v);

        int AddOne(int v);

        int AddOne2(int v);

        int AddOne3(int v);
    }

    public class FateContext
    {
        public IServiceProvider Provider { get; set; }
        public MethodInfo Method { get; set; }
    }

    public class FateContext<T> : FateContext
    {
        public T[] Parameters { get; set; }
        public T ReturnValue { get; set; }
    }

    public class FateContextTask<T> : FateContext
    {
        public T[] Parameters { get; set; }
        public Task<T> ReturnValue { get; set; }
    }

    public class TestInterceptor : IInterceptor
    {
        public virtual async Task InvokeAsync(FateContext context, InterceptAsync next)
        {
            var c = context as FateContextTask<int>;
            await next(context);
            c.ReturnValue = Task.FromResult(1 + c.ReturnValue.Result);
        }

        //public virtual void Invoke(FateContext context, Intercept next)
        //{
        //    Task InvokeNextAsync(FateContext c)
        //    {
        //        next(c);
        //        c.ReturnValue = Task.FromResult((int)c.ReturnValue);
        //        return Task.CompletedTask;
        //    }

        //    InvokeAsync(context, InvokeNextAsync).ConfigureAwait(false).GetAwaiter().GetResult();
        //}

        public virtual void Invoke(FateContext context, Intercept next)
        {
            var c = context as FateContext<int>;
            next(context);
            c.ReturnValue = 1 + c.ReturnValue;
        }
    }

    public class TestInterceptor2 : TestInterceptor
    {
        public override void Invoke(FateContext context, Intercept next)
        {
            var c = context as FateContext<int>;
            next(context);
            c.ReturnValue = 1 + c.ReturnValue;
        }
    }

    public class Test : ITest
    {
        public virtual int AddOne(int v)
        {
            return v + 1;
        }

        public virtual int AddOne2(int v)
        {
            return v + 1;
        }

        public virtual int AddOne3(int v)
        {
            return v + 1;
        }

        public virtual Task<int> AddOneAsync(int v)
        {
            return Task.FromResult(v + 1);
        }
    }

    public class TestProxy : Test, IInterceptProxy
    {
        public IServiceProvider Provider { get; set; }

        private readonly IInterceptor Interceptor;
        private readonly IInterceptor Interceptor2;

        public TestProxy(IServiceProvider provider) : base()
        {
            Provider = provider;
            Interceptor = new TestInterceptor();
            Interceptor2 = new TestInterceptor2();
        }

        public override async Task<int> AddOneAsync(int v)
        {
            async Task InvokeBaseAsync(FateContext c)
            {
                var d = ((FateContextTask<int>)c);
                var task = base.AddOneAsync(d.Parameters[0]);
                d.ReturnValue = task;
                await task;
            }
            var context = new FateContextTask<int>();
            context.Parameters = new int[] { v };
            await Interceptor.InvokeAsync(context, InvokeBaseAsync);
            return context.ReturnValue.Result;
        }

        public override int AddOne(int v)
        {
            void InvokeBase(FateContext c)
            {
                var d = ((FateContext<int>)c);
                var result = base.AddOne(d.Parameters[0]);
                d.ReturnValue = result;
            }
            var context = new FateContext<int>();
            context.Parameters = new int[] { v };
            Interceptor.Invoke(context, InvokeBase);
            return context.ReturnValue;
        }

        public override int AddOne2(int v)
        {
            void InvokeBase(FateContext c)
            {
                var d = ((FateContext<int>)c);
                var result = base.AddOne2(d.Parameters[0]);
                d.ReturnValue = result;
            }
            var context = new FateContext<int>();
            context.Parameters = new int[] { v };
            Interceptor2.Invoke(context, InvokeBase);
            return context.ReturnValue;
        }

        public override int AddOne3(int v)
        {
            int r;
            //try
            //{
                r = base.AddOne3(v);
                r = r + 1;
            //}
            //catch
            //{
            //    throw;
            //}
            return r;
        }
    }

    [SimpleJob(RuntimeMoniker.NetCoreApp31)]
    [RPlotExporter]
    public class InterceptorBenchmarkTest
    {
        private readonly ITest proxy = new TestProxy(null);
        private readonly ITest test = new Test();
        private const int Count = 100000;

        [Benchmark]
        public async Task RealCallAsync()
        {
            for (int i = 0; i < Count; i++)
            {
                await test.AddOneAsync(i);
            }
        }

        [Benchmark]
        public async Task ProxyCallAsync()
        {
            for (int i = 0; i < Count; i++)
            {
                await proxy.AddOneAsync(i);
            }
        }

        [Benchmark]
        public void RealCallSync()
        {
            for (int i = 0; i < Count; i++)
            {
                test.AddOne(i);
            }
        }

        [Benchmark]
        public void ProxyCallSyncOnAsync()
        {
            for (int i = 0; i < Count; i++)
            {
                proxy.AddOne(i);
            }
        }

        [Benchmark]
        public void ProxyCallSync()
        {
            for (int i = 0; i < Count; i++)
            {
                proxy.AddOne2(i);
            }
        }

        [Benchmark]
        public void ProxyCallStaticSync()
        {
            for (int i = 0; i < Count; i++)
            {
                proxy.AddOne3(i);
            }
        }

        public async Task Test()
        {
            var sw = Stopwatch.StartNew(); 
            await RealCallAsync();
            sw.Stop();
            Console.WriteLine($"RealCallAsync : {sw.ElapsedMilliseconds}");

            sw.Restart();
            await ProxyCallAsync();
            sw.Stop();
            Console.WriteLine($"ProxyCallAsync : {sw.ElapsedMilliseconds}");

            sw.Restart();
            RealCallSync();
            sw.Stop();
            Console.WriteLine($"RealCallSync : {sw.ElapsedMilliseconds}");

            sw.Restart();
            ProxyCallSyncOnAsync();
            sw.Stop();
            Console.WriteLine($"ProxyCallSyncOnAsync : {sw.ElapsedMilliseconds}");

            sw.Restart();
            ProxyCallSync();
            sw.Stop();
            Console.WriteLine($"ProxyCallSync : {sw.ElapsedMilliseconds}");

            sw.Restart();
            ProxyCallStaticSync();
            sw.Stop();
            Console.WriteLine($"ProxyCallStaticSync : {sw.ElapsedMilliseconds}");
        }
    }
}
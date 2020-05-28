using Microsoft.Extensions.DependencyInjection;
using Norns.Adapters.DependencyInjection.Attributes;
using Norns.Fate.Abstraction;
using Norns.UT.Fate;
using System;
using System.Threading.Tasks;
using Xunit;

[assembly: ProxyMapping(typeof(ITest), typeof(Test), typeof(TestProxy))]
[assembly: ProxyMapping(typeof(ITest), typeof(TestInterceptor), typeof(TestProxy))]

namespace Norns.UT.Fate
{
    public interface ITest
    {
        Task<int> AddOneAsync(int v);

        int AddOne(int v);
    }

    public class TestInterceptor : IInterceptor
    {
        public async Task InvokeAsync(FateContext context, InterceptAsync next)
        {
            await next(context);
            context.ReturnValue = 1 + (context.ReturnValue as Task<int>).Result;
        }

        public void Invoke(FateContext context, Intercept next)
        {
            Task InvokeNextAsync(FateContext c)
            {
                next(c);
                c.ReturnValue = Task.FromResult((int)c.ReturnValue);
                return Task.CompletedTask;
            }

            InvokeAsync(context, InvokeNextAsync).ConfigureAwait(false).GetAwaiter().GetResult();
        }

        //public void Invoke(FateContext context, Intercept next)
        //{
        //    next(context);
        //    context.ReturnValue = 1 + (int)context.ReturnValue;
        //}
    }

    public class Test : ITest
    {
        public int AddOne(int v)
        {
            return v + 1;
        }

        public Task<int> AddOneAsync(int v)
        {
            return Task.FromResult(v + 1);
        }
    }

    public class TestProxy : ITest, IInterceptProxy
    {
        public IServiceProvider Provider { get; set; }

        private readonly IInterceptor Interceptor;
        private readonly Test test;

        public TestProxy(IServiceProvider provider, Test test)
        {
            Provider = provider;
            this.test = test;
            Interceptor = new TestInterceptor();
        }

        public async Task<int> AddOneAsync(int v)
        {
            async Task InvokeBaseAsync(FateContext c)
            {
                var task = test.AddOneAsync((int)c.Parameters[0]);
                c.ReturnValue = task;
                await task;
            }
            var context = new FateContext();
            context.Parameters = new object[] { v };
            await Interceptor.InvokeAsync(context, InvokeBaseAsync);
            return (int)context.ReturnValue;
        }

        public int AddOne(int v)
        {
            void InvokeBase(FateContext c)
            {
                var result = test.AddOne((int)c.Parameters[0]);
                c.ReturnValue = result;
            }
            var context = new FateContext();
            context.Parameters = new object[] { v };
            Interceptor.Invoke(context, InvokeBase);
            return (int)context.ReturnValue;
        }
    }

    public class InterceptorTest
    {
        [Fact]
        public async Task AddOneAsyncShouldAddTwo()
        {
            var p = new ServiceCollection()
                .AddSingleton<ITest, Test>()
                .BuildAopServiceProvider();

            var proxy = p.GetRequiredService<ITest>();
            Assert.Equal(5, await proxy.AddOneAsync(3));
        }

        //[Fact]
        //public void AddOneShouldAddTwo()
        //{
        //    var proxy = new TestProxy(null);
        //    Assert.Equal(5, proxy.AddOne(3));
        //}
    }
}
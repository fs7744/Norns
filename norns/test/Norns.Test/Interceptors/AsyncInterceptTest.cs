using Norns.AOP.Attributes;
using Norns.AOP.Core.Interceptors;
using Norns.AOP.Interceptors;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Norns.Test.Interceptors
{
    public class AsyncInterceptTest
    {
        [AddOne]
        public interface ITestSumService
        {
            ValueTask<int> SumAsync(int x, int y);
        }

        public class TestSumService : ITestSumService
        {
            public virtual ValueTask<int> SumAsync(int x, int y)
            {
                return new ValueTask<int>(x + y);
            }
        }

        public class AddOneAttribute : InterceptorBaseAttribute
        {
            public override async Task InterceptAsync(InterceptContext context, AsyncInterceptorDelegate nextAsync)
            {
                await nextAsync(context);
                context.Result = (int)context.Result + 1;
            }
        }

        public class AddOne2Attribute : InterceptorBase
        {
            public override async Task InterceptAsync(InterceptContext context, AsyncInterceptorDelegate nextAsync)
            {
                await nextAsync(context);
                context.Result = (int)context.Result + 1;
            }
        }

        public class TestSumServiceProxy : TestSumService
        {
            private readonly MethodInfo sumMethod;
            private readonly AsyncInterceptorDelegate sumInterceptor;

            public TestSumServiceProxy(IInterceptorBuilder builder)
            {
                sumMethod = typeof(TestSumService).GetMethod("SumAsync", new Type[] { typeof(int), typeof(int) });
                sumInterceptor = builder.BuildAsyncInterceptor(sumMethod, async c =>
                {
                    c.Result = await base.SumAsync((int)c.Parameters[0], (int)c.Parameters[1]);
                });
            }

            public override async ValueTask<int> SumAsync(int x, int y)
            {
                var context = new InterceptContext()
                {
                    ServiceMethod = sumMethod,
                    Parameters = new object[] { x, y }
                };
                await sumInterceptor(context);
                return (int)context.Result;
            }
        }

        [Fact]
        public async Task WhenSum6And3ThenAddOneInterceptorShouldBe10()
        {
            var result = await new TestSumServiceProxy(new InterceptorBuilder(new IInterceptor[] { new AddOneAttribute() })).SumAsync(6, 3);
            Assert.Equal(10, result);
        }

        [Fact]
        public async Task WhenSum6And3ThenAddOne2InterceptorShouldBe10()
        {
            var result = await new TestSumServiceProxy(new InterceptorBuilder(new IInterceptor[] { new AddOne2Attribute() })).SumAsync(6, 3);
            Assert.Equal(10, result);
        }
    }
}
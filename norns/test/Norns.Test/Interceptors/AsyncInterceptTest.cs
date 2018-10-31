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
            public override async Task InterceptAsync(InterceptContext context, AsyncInterceptDelegate nextAsync)
            {
                await nextAsync(context);
                context.Result = (int)context.Result + 1;
            }
        }

        public class AddOne2Attribute : InterceptorBase
        {
            public override async Task InterceptAsync(InterceptContext context, AsyncInterceptDelegate nextAsync)
            {
                await nextAsync(context);
                context.Result = (int)context.Result + 1;
            }
        }

        public class TestSumServiceProxy : TestSumService
        {
            private readonly MethodInfo sumMethod;
            private readonly AsyncInterceptDelegate sumInterceptor;

            public TestSumServiceProxy(IInterceptDelegateBuilder builder)
            {
                sumMethod = typeof(TestSumService).GetMethod("SumAsync", new Type[] { typeof(int), typeof(int) });
                sumInterceptor = builder.BuildAsyncInterceptDelegate(sumMethod, async c =>
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
            var verifiers = new IInterceptBox[]
            { new InterceptBox(new AddOneAttribute(), m => true) };
            var result = await new TestSumServiceProxy(new InterceptDelegateBuilder(verifiers)).SumAsync(6, 3);
            Assert.Equal(10, result);
        }

        [Fact]
        public async Task WhenSum6And3ThenAddOne2InterceptorShouldBe10()
        {
            var verifiers = new IInterceptBox[]
            { new InterceptBox(new AddOne2Attribute(), m => true) };
            var result = await new TestSumServiceProxy(new InterceptDelegateBuilder(verifiers)).SumAsync(6, 3);
            Assert.Equal(10, result);
        }
    }
}
using Norns.AOP.Attributes;
using Norns.AOP.Core.Interceptors;
using Norns.AOP.Interceptors;
using System;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace Norns.Test.Interceptors
{
    public class SyncInterceptTest
    {
        [AddOne]
        public interface ITestSumService
        {
            int Sum(int x, int y);
        }

        public class TestSumService : ITestSumService
        {
            public virtual int Sum(int x, int y)
            {
                return x + y;
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

        public class AddOneSyncAttribute : InterceptorBaseAttribute
        {
            public override void Intercept(InterceptContext context, InterceptDelegate next)
            {
                next(context);
                context.Result = (int)context.Result + 1;
            }

            public override async Task InterceptAsync(InterceptContext context, AsyncInterceptDelegate nextAsync)
            {
                throw new NotImplementedException();
            }
        }

        public class AddOneSync2Attribute : InterceptorBase
        {
            public override void Intercept(InterceptContext context, InterceptDelegate next)
            {
                next(context);
                context.Result = (int)context.Result + 1;
            }

            public override async Task InterceptAsync(InterceptContext context, AsyncInterceptDelegate nextAsync)
            {
                throw new NotImplementedException();
            }
        }

        public class TestSumServiceProxy : TestSumService
        {
            private readonly MethodInfo sumMethod;
            private readonly InterceptDelegate sumInterceptor;

            public TestSumServiceProxy(IInterceptDelegateBuilder builder)
            {
                sumMethod = typeof(TestSumService).GetMethod("Sum", new Type[] { typeof(int), typeof(int) });
                sumInterceptor = builder.BuildInterceptDelegate(sumMethod, c =>
                {
                    c.Result = base.Sum((int)c.Parameters[0], (int)c.Parameters[1]);
                });
            }

            public override int Sum(int x, int y)
            {
                var context = new InterceptContext()
                {
                    ServiceMethod = sumMethod,
                    Parameters = new object[] { x, y }
                };
                sumInterceptor(context);
                return (int)context.Result;
            }
        }

        [Fact]
        public void WhenSum6And3ThenAddOneInterceptorShouldBe10()
        {
            var verifiers = new IInterceptBox[]
            { new InterceptBox(new AddOneAttribute(), m => true) };
            var result = new TestSumServiceProxy(new InterceptDelegateBuilder(verifiers)).Sum(6, 3);
            Assert.Equal(10, result);
        }

        [Fact]
        public void WhenSum6And3ThenAddOneSyncInterceptorShouldBe10()
        {
            var verifiers = new IInterceptBox[]
            { new InterceptBox(new AddOneSyncAttribute(), m => true) };
            var result = new TestSumServiceProxy(new InterceptDelegateBuilder(verifiers)).Sum(6, 3);
            Assert.Equal(10, result);
        }

        [Fact]
        public void WhenSum6And3ThenAddOne2InterceptorShouldBe10()
        {
            var verifiers = new IInterceptBox[]
            { new InterceptBox(new AddOne2Attribute(), m => true) };
            var result = new TestSumServiceProxy(new InterceptDelegateBuilder(verifiers)).Sum(6, 3);
            Assert.Equal(10, result);
        }

        [Fact]
        public void WhenSum6And3ThenAddOneSync2InterceptorShouldBe10()
        {
            var verifiers = new IInterceptBox[]
            { new InterceptBox(new AddOneSync2Attribute(), m => true) };
            var result = new TestSumServiceProxy(new InterceptDelegateBuilder(verifiers)).Sum(6, 3);
            Assert.Equal(10, result);
        }
    }
}
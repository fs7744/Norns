using Norns.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Norns.Test.DependencyInjection
{
    public class ScopedTest
    {
        public class TestScopedClass
        {
            private TestScopedClass()
            {
            }

            public TestScopedClass(ScopedTest scoped, IServiceProvider provider)
            {
                Scoped = scoped;
            }

            public ScopedTest Scoped { get; }
        }

        public class TestScopedGenericClass<T>
        {
            private TestScopedGenericClass()
            {
            }

            public TestScopedGenericClass(T scoped)
            {
                Scoped = scoped;
            }

            public T Scoped { get; }
        }

        [Fact]
        public void ScopedWhenFactory()
        {
            var service = new ServiceDefintions()
                .AddScoped(i => this)
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(ScopedTest));
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(ScopedTest));
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void ScopedWhenTypeFactory()
        {
            var service = new ServiceDefintions()
                .AddScoped<ScopedTest, ScopedTest>(i => this)
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(ScopedTest));
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(ScopedTest));
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void ScopedWhenServiceType()
        {
            var service = new ServiceDefintions()
                .AddScoped<ScopedTest, ScopedTest>()
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(ScopedTest));
            Assert.NotSame(this, result0);
            var result1 = service.GetService(typeof(ScopedTest));
            Assert.NotSame(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void ScopedWhenServiceTypeAndHasParameters()
        {
            var service = new ServiceDefintions()
                .AddScoped<ScopedTest, ScopedTest>()
                .AddScoped<TestScopedClass>()
                .BuildServiceProvider();

            var result0 = (TestScopedClass)service.GetService(typeof(TestScopedClass));
            Assert.NotNull(result0);
            Assert.NotSame(this, result0.Scoped);
            var result1 = (TestScopedClass)service.GetService(typeof(TestScopedClass));
            Assert.NotNull(result1);
            Assert.NotSame(this, result1.Scoped);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void ScopedWhenServiceTypeAndHasGenericParameters()
        {
            var service = new ServiceDefintions()
                .AddScoped<ScopedTest, ScopedTest>()
                .AddScoped<TestScopedGenericClass<ScopedTest>>()
                .BuildServiceProvider();

            var result0 = (TestScopedGenericClass<ScopedTest>)service.GetService(typeof(TestScopedGenericClass<ScopedTest>));
            Assert.NotNull(result0);
            Assert.NotSame(this, result0.Scoped);
            var result1 = (TestScopedGenericClass<ScopedTest>)service.GetService(typeof(TestScopedGenericClass<ScopedTest>));
            Assert.NotNull(result1);
            Assert.NotSame(this, result1.Scoped);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void ScopedWhenGenericServiceTypeAndHasGenericParameters()
        {
            var service = new ServiceDefintions()
                .AddScoped<ScopedTest, ScopedTest>()
                .AddScoped(typeof(TestScopedGenericClass<>))
                .BuildServiceProvider();

            var result0 = (TestScopedGenericClass<ScopedTest>)service.GetService(typeof(TestScopedGenericClass<ScopedTest>));
            Assert.NotNull(result0);
            Assert.NotSame(this, result0.Scoped);
            var result1 = (TestScopedGenericClass<ScopedTest>)service.GetService(typeof(TestScopedGenericClass<ScopedTest>));
            Assert.NotNull(result1);
            Assert.NotSame(this, result1.Scoped);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void ScopedWhenIEnumerableInt()
        {
            var service = new ServiceDefintions()
                .AddScoped<IEnumerable<int>, List<int>>()
                .BuildServiceProvider();

            var result0 = (List<int>)service.GetService(typeof(IEnumerable<int>));
            Assert.NotNull(result0);
            var result1 = (List<int>)service.GetService(typeof(IEnumerable<int>));
            Assert.NotNull(result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void ScopedWhenObjectAndGetIEnumerableObject()
        {
            var service = new ServiceDefintions()
                .AddScoped(i => this)
                .BuildServiceProvider();

            var resultList = (IEnumerable<ScopedTest>)service.GetService(typeof(IEnumerable<ScopedTest>));
            var result0 = resultList.ToArray();
            Assert.Single(resultList);
            Assert.Same(this, result0[0]);
            var resultList1 = (IEnumerable<ScopedTest>)service.GetService(typeof(IEnumerable<ScopedTest>));
            var result1 = resultList1.ToArray();
            Assert.Single(resultList1);
            Assert.Same(this, result1[0]);
            Assert.NotSame(resultList, resultList1);
            Assert.Same(result0[0], result1[0]);
        }

        [Fact]
        public void ScopedWhenObjectAndGetIEnumerableInt()
        {
            var service = new ServiceDefintions()
                .AddScoped<IEnumerable<int>, List<int>>()
                .BuildServiceProvider();

            var resultList = (IEnumerable<int>)service.GetService(typeof(IEnumerable<int>));
            Assert.Empty(resultList);
            var resultList1 = (IEnumerable<int>)service.GetService(typeof(IEnumerable<int>));
            Assert.Empty(resultList1);
            Assert.Same(resultList, resultList1);
        }

        [Fact]
        public void ScopedWhenObjectAndGetIEnumerableIntWithNewScoped()
        {
            var service = new ServiceDefintions()
                .AddScoped<IEnumerable<int>, List<int>>()
                .BuildServiceProvider();

            var resultList = service.GetService<IEnumerable<int>>();
            Assert.Empty(resultList);
            var resultList1 = service.GetRequiredService<IEnumerable<int>>();
            Assert.Empty(resultList1);
            Assert.Same(resultList, resultList1);

            var provider = service.GetRequiredService<IServiceScopeFactory>().CreateScopeProvider();
            using (provider as IDisposable)
            {
                var resultList2 = provider.GetRequiredService<IEnumerable<int>>();
                Assert.Empty(resultList2);
                Assert.NotSame(resultList, resultList2);
                Assert.NotSame(resultList1, resultList2);
            }
            Assert.Throws<ObjectDisposedException>(() => provider.GetRequiredService<IEnumerable<int>>());
        }

        [Fact]
        public void ScopedWhenObjectAndGetIEnumerableList()
        {
            var service = new ServiceDefintions()
                .AddScoped(i => this)
                .BuildServiceProvider();

            var resultList = service.GetRequiredServices<IEnumerable<ScopedTest>>();
            var result00 = resultList.ToArray();
            var result0 = result00[0].ToArray();
            Assert.Single(resultList);
            Assert.Single(result0);
            Assert.Same(this, result0[0]);
            var resultList1 = service.GetServices<IEnumerable<ScopedTest>>();
            var result11 = resultList1.ToArray();
            var result1 = result11[0].ToArray();
            Assert.Single(resultList1);
            Assert.Single(result1);
            Assert.Same(this, result1[0]);
            Assert.NotSame(resultList, resultList1);
            Assert.Same(result0[0], result1[0]);
        }
    }
}
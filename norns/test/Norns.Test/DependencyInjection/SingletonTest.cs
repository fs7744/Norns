using Norns.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Norns.Test.DependencyInjection
{
    public class SingletonTest
    {
        public class TestSingletonClass
        {
            private TestSingletonClass()
            {
            }

            public TestSingletonClass(SingletonTest singleton)
            {
                Singleton = singleton;
            }

            public SingletonTest Singleton { get; }
        }

        public class TestSingletonGenericClass<T>
        {
            private TestSingletonGenericClass()
            {
            }

            public TestSingletonGenericClass(T singleton)
            {
                Singleton = singleton;
            }

            public T Singleton { get; }
        }

        [Fact]
        public void SingletonWhenObject()
        {
            var service = new ServiceDefintions()
                .AddSingleton(this)
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenTypeObject()
        {
            var service = new ServiceDefintions()
                .AddSingleton<SingletonTest, SingletonTest>(this)
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenFactory()
        {
            var service = new ServiceDefintions()
                .AddSingleton(i => this)
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenTypeFactory()
        {
            var service = new ServiceDefintions()
                .AddSingleton<SingletonTest, SingletonTest>(i => this)
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(SingletonTest));
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenServiceType()
        {
            var service = new ServiceDefintions()
                .AddSingleton<SingletonTest, SingletonTest>()
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(SingletonTest));
            Assert.NotSame(this, result0);
            var result1 = service.GetService(typeof(SingletonTest));
            Assert.NotSame(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenServiceTypeAndHasParameters()
        {
            var service = new ServiceDefintions()
                .AddSingleton<SingletonTest, SingletonTest>()
                .AddSingleton<TestSingletonClass>()
                .BuildServiceProvider();

            var result0 = (TestSingletonClass)service.GetService(typeof(TestSingletonClass));
            Assert.NotNull(result0);
            Assert.NotSame(this, result0.Singleton);
            var result1 = (TestSingletonClass)service.GetService(typeof(TestSingletonClass));
            Assert.NotNull(result1);
            Assert.NotSame(this, result1.Singleton);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenServiceTypeAndHasGenericParameters()
        {
            var service = new ServiceDefintions()
                .AddSingleton<SingletonTest, SingletonTest>()
                .AddSingleton<TestSingletonGenericClass<SingletonTest>>()
                .BuildServiceProvider();

            var result0 = (TestSingletonGenericClass<SingletonTest>)service.GetService(typeof(TestSingletonGenericClass<SingletonTest>));
            Assert.NotNull(result0);
            Assert.NotSame(this, result0.Singleton);
            var result1 = (TestSingletonGenericClass<SingletonTest>)service.GetService(typeof(TestSingletonGenericClass<SingletonTest>));
            Assert.NotNull(result1);
            Assert.NotSame(this, result1.Singleton);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenGenericServiceTypeAndHasGenericParameters()
        {
            var service = new ServiceDefintions()
                .AddSingleton<SingletonTest, SingletonTest>()
                .AddSingleton(typeof(TestSingletonGenericClass<>))
                .BuildServiceProvider();

            var result0 = (TestSingletonGenericClass<SingletonTest>)service.GetService(typeof(TestSingletonGenericClass<SingletonTest>));
            Assert.NotNull(result0);
            Assert.NotSame(this, result0.Singleton);
            var result1 = (TestSingletonGenericClass<SingletonTest>)service.GetService(typeof(TestSingletonGenericClass<SingletonTest>));
            Assert.NotNull(result1);
            Assert.NotSame(this, result1.Singleton);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenIEnumerableInt()
        {
            var service = new ServiceDefintions()
                .AddSingleton<IEnumerable<int>, List<int>>()
                .BuildServiceProvider();

            var result0 = (List<int>)service.GetService(typeof(IEnumerable<int>));
            Assert.NotNull(result0);
            var result1 = (List<int>)service.GetService(typeof(IEnumerable<int>));
            Assert.NotNull(result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void SingletonWhenObjectAndGetIEnumerableObject()
        {
            var service = new ServiceDefintions()
                .AddSingleton(this)
                .BuildServiceProvider();

            var resultList = (IEnumerable<SingletonTest>)service.GetService(typeof(IEnumerable<SingletonTest>));
            var result0 = resultList.ToArray();
            Assert.Single(resultList);
            Assert.Same(this, result0[0]);
            var resultList1 = (IEnumerable<SingletonTest>)service.GetService(typeof(IEnumerable<SingletonTest>));
            var result1 = resultList1.ToArray();
            Assert.Single(resultList1);
            Assert.Same(this, result1[0]);
            Assert.NotSame(resultList, resultList1);
            Assert.Same(result0[0], result1[0]);
        }

        [Fact]
        public void SingletonWhenObjectAndGetIEnumerableInt()
        {
            var service = new ServiceDefintions()
                .AddSingleton<IEnumerable<int>, List<int>>()
                .BuildServiceProvider();

            var resultList = (IEnumerable<int>)service.GetService(typeof(IEnumerable<int>));
            Assert.Empty(resultList);
            var resultList1 = (IEnumerable<int>)service.GetService(typeof(IEnumerable<int>));
            Assert.Empty(resultList1);
            Assert.Same(resultList, resultList1);
        }

        [Fact]
        public void SingletonWhenObjectAndGetIEnumerableList()
        {
            var service = new ServiceDefintions()
                .AddSingleton(this)
                .BuildServiceProvider();

            var resultList = (IEnumerable<IEnumerable<SingletonTest>>)service.GetService(typeof(IEnumerable<IEnumerable<SingletonTest>>));
            var result00 = resultList.ToArray();
            var result0 = result00[0].ToArray();
            Assert.Single(resultList);
            Assert.Single(result0);
            Assert.Same(this, result0[0]);
            var resultList1 = (IEnumerable<IEnumerable<SingletonTest>>)service.GetService(typeof(IEnumerable<IEnumerable<SingletonTest>>));
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
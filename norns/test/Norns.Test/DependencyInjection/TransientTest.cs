using Norns.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Norns.Test.DependencyInjection
{
    public class TransientTest
    {
        public class TestTransientClass
        {
            private TestTransientClass()
            {
            }

            public TestTransientClass(TransientTest transient)
            {
                Transient = transient;
            }

            public TransientTest Transient { get; }
        }

        public class TestTransientGenericClass<T>
        {
            private TestTransientGenericClass()
            {
            }

            public TestTransientGenericClass(T transient)
            {
                Transient = transient;
            }

            public T Transient { get; }
        }

        [Fact]
        public void TransientWhenFactory()
        {
            var service = new ServiceDefintions()
                .AddTransient(i => this)
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(TransientTest));
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(TransientTest));
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void TransientWhenTypeFactory()
        {
            var service = new ServiceDefintions()
                .AddTransient<TransientTest, TransientTest>(i => this)
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(TransientTest));
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(TransientTest));
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void TransientWhenServiceType()
        {
            var service = new ServiceDefintions()
                .AddTransient<TransientTest, TransientTest>()
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(TransientTest));
            Assert.NotSame(this, result0);
            var result1 = service.GetService(typeof(TransientTest));
            Assert.NotSame(this, result1);
            Assert.NotSame(result0, result1);
        }

        [Fact]
        public void TransientWhenServiceTypeAndHasParameters()
        {
            var service = new ServiceDefintions()
                .AddTransient<TransientTest, TransientTest>()
                .AddTransient<TestTransientClass>()
                .BuildServiceProvider();

            var result0 = (TestTransientClass)service.GetService(typeof(TestTransientClass));
            Assert.NotNull(result0);
            Assert.NotSame(this, result0.Transient);
            var result1 = (TestTransientClass)service.GetService(typeof(TestTransientClass));
            Assert.NotNull(result1);
            Assert.NotSame(this, result1.Transient);
            Assert.NotSame(result0, result1);
        }

        [Fact]
        public void TransientWhenServiceTypeAndHasGenericParameters()
        {
            var service = new ServiceDefintions()
                .AddTransient<TransientTest, TransientTest>()
                .AddTransient<TestTransientGenericClass<TransientTest>>()
                .BuildServiceProvider();

            var result0 = (TestTransientGenericClass<TransientTest>)service.GetService(typeof(TestTransientGenericClass<TransientTest>));
            Assert.NotNull(result0);
            Assert.NotSame(this, result0.Transient);
            var result1 = (TestTransientGenericClass<TransientTest>)service.GetService(typeof(TestTransientGenericClass<TransientTest>));
            Assert.NotNull(result1);
            Assert.NotSame(this, result1.Transient);
            Assert.NotSame(result0, result1);
        }

        [Fact]
        public void TransientWhenGenericServiceTypeAndHasGenericParameters()
        {
            var service = new ServiceDefintions()
                .AddTransient<TransientTest, TransientTest>()
                .AddTransient(typeof(TestTransientGenericClass<>))
                .BuildServiceProvider();

            var result0 = (TestTransientGenericClass<TransientTest>)service.GetService(typeof(TestTransientGenericClass<TransientTest>));
            Assert.NotNull(result0);
            Assert.NotSame(this, result0.Transient);
            var result1 = (TestTransientGenericClass<TransientTest>)service.GetService(typeof(TestTransientGenericClass<TransientTest>));
            Assert.NotNull(result1);
            Assert.NotSame(this, result1.Transient);
            Assert.NotSame(result0, result1);
        }

        [Fact]
        public void TransientWhenIEnumerableInt()
        {
            var service = new ServiceDefintions()
                .AddTransient<IEnumerable<int>, List<int>>()
                .BuildServiceProvider();

            var result0 = (List<int>)service.GetService(typeof(IEnumerable<int>));
            Assert.NotNull(result0);
            var result1 = (List<int>)service.GetService(typeof(IEnumerable<int>));
            Assert.NotNull(result1);
            Assert.NotSame(result0, result1);
        }

        [Fact]
        public void TransientWhenObjectAndGetIEnumerableObject()
        {
            var service = new ServiceDefintions()
                .AddTransient(i => this)
                .BuildServiceProvider();

            var resultList = (IEnumerable<TransientTest>)service.GetService(typeof(IEnumerable<TransientTest>));
            var result0 = resultList.ToArray();
            Assert.Single(resultList);
            Assert.Same(this, result0[0]);
            var resultList1 = (IEnumerable<TransientTest>)service.GetService(typeof(IEnumerable<TransientTest>));
            var result1 = resultList1.ToArray();
            Assert.Single(resultList1);
            Assert.Same(this, result1[0]);
            Assert.NotSame(resultList, resultList1);
            Assert.Same(result0[0], result1[0]);
        }

        [Fact]
        public void TransientWhenObjectAndGetIEnumerableInt()
        {
            var service = new ServiceDefintions()
                .AddTransient<IEnumerable<int>, List<int>>()
                .BuildServiceProvider();

            var resultList = (IEnumerable<int>)service.GetService(typeof(IEnumerable<int>));
            Assert.Empty(resultList);
            var resultList1 = (IEnumerable<int>)service.GetService(typeof(IEnumerable<int>));
            Assert.Empty(resultList1);
            Assert.NotSame(resultList, resultList1);
        }

        [Fact]
        public void TransientWhenObjectAndGetIEnumerableList()
        {
            var service = new ServiceDefintions()
                .AddTransient(i => this)
                .BuildServiceProvider();

            var resultList = (IEnumerable<IEnumerable<TransientTest>>)service.GetService(typeof(IEnumerable<IEnumerable<TransientTest>>));
            var result00 = resultList.ToArray();
            var result0 = result00[0].ToArray();
            Assert.Single(resultList);
            Assert.Single(result0);
            Assert.Same(this, result0[0]);
            var resultList1 = (IEnumerable<IEnumerable<TransientTest>>)service.GetService(typeof(IEnumerable<IEnumerable<TransientTest>>));
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
using Norns.DependencyInjection;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace Norns.Test.DependencyInjection
{
    public class NamedTransientTest
    {
        public class TestTransientClass
        {
            private TestTransientClass()
            {
            }

            public TestTransientClass([FromDI(Named = "T")]NamedTransientTest transient)
            {
                Transient = transient;
            }

            public NamedTransientTest Transient { get; }
        }

        public class TestTransientGenericClass<T>
        {
            private TestTransientGenericClass()
            {
            }

            public TestTransientGenericClass([FromDI(Named = "T")]T transient)
            {
                Transient = transient;
            }

            public T Transient { get; }
        }

        [Fact]
        public void TransientWhenFactory()
        {
            var service = new ServiceDefintions()
                .AddTransient(i => this, "T")
                .AddSingleton(i => new NamedTransientTest())
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(NamedTransientTest), "T");
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(NamedTransientTest), "T");
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void TransientWhenTypeFactory()
        {
            var service = new ServiceDefintions()
                .AddTransient<NamedTransientTest, NamedTransientTest>(i => this, "T")
                .AddSingleton<NamedTransientTest, NamedTransientTest>(i => new NamedTransientTest())
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(NamedTransientTest), "T");
            Assert.Same(this, result0);
            var result1 = service.GetService(typeof(NamedTransientTest), "T");
            Assert.Same(this, result1);
            Assert.Same(result0, result1);
        }

        [Fact]
        public void TransientWhenServiceType()
        {
            var service = new ServiceDefintions()
                .AddTransient<TransientTest, TransientTest>("T")
                .AddSingleton<TransientTest, TransientTest>()
                .BuildServiceProvider();

            var result0 = service.GetService(typeof(TransientTest), "T");
            Assert.NotSame(this, result0);
            var result1 = service.GetService(typeof(TransientTest), "T");
            Assert.NotSame(this, result1);
            Assert.NotSame(result0, result1);
        }

        [Fact]
        public void TransientWhenServiceTypeAndHasParameters()
        {
            var service = new ServiceDefintions()
                .AddTransient<TransientTest, TransientTest>("T")
                .AddTransient<TestTransientClass>("T")
                .AddSingleton<TransientTest, TransientTest>()
                .AddSingleton<TestTransientClass>()
                .BuildServiceProvider();

            var result0 = (TestTransientClass)service.GetService(typeof(TestTransientClass), "T");
            Assert.NotNull(result0);
            Assert.NotSame(this, result0.Transient);
            var result1 = (TestTransientClass)service.GetService(typeof(TestTransientClass), "T");
            Assert.NotNull(result1);
            Assert.NotSame(this, result1.Transient);
            Assert.NotSame(result0, result1);
        }

        [Fact]
        public void TransientWhenServiceTypeAndHasGenericParameters()
        {
            var service = new ServiceDefintions()
                .AddTransient<TransientTest, TransientTest>("T")
                .AddTransient<TestTransientGenericClass<TransientTest>>("T")
                .AddSingleton<TransientTest, TransientTest>()
                .AddSingleton<TestTransientGenericClass<TransientTest>>()
                .BuildServiceProvider();

            var result0 = (TestTransientGenericClass<TransientTest>)service.GetService(typeof(TestTransientGenericClass<TransientTest>), "T");
            Assert.NotNull(result0);
            Assert.NotSame(this, result0.Transient);
            var result1 = (TestTransientGenericClass<TransientTest>)service.GetService(typeof(TestTransientGenericClass<TransientTest>), "T");
            Assert.NotNull(result1);
            Assert.NotSame(this, result1.Transient);
            Assert.NotSame(result0, result1);
        }

        [Fact]
        public void TransientWhenGenericServiceTypeAndHasGenericParameters()
        {
            var service = new ServiceDefintions()
                .AddTransient<TransientTest, TransientTest>("T")
                .AddTransient(typeof(TestTransientGenericClass<>), "T")
                .AddSingleton<TransientTest, TransientTest>()
                .AddSingleton(typeof(TestTransientGenericClass<>))
                .BuildServiceProvider();

            var result0 = (TestTransientGenericClass<TransientTest>)service.GetService(typeof(TestTransientGenericClass<TransientTest>), "T");
            Assert.NotNull(result0);
            Assert.NotSame(this, result0.Transient);
            var result1 = (TestTransientGenericClass<TransientTest>)service.GetService(typeof(TestTransientGenericClass<TransientTest>), "T");
            Assert.NotNull(result1);
            Assert.NotSame(this, result1.Transient);
            Assert.NotSame(result0, result1);
        }

        [Fact]
        public void TransientWhenIEnumerableInt()
        {
            var service = new ServiceDefintions()
                .AddTransient<IEnumerable<int>, List<int>>("T")
                .AddSingleton<IEnumerable<int>, List<int>>()
                .BuildServiceProvider();

            var result0 = (List<int>)service.GetService(typeof(IEnumerable<int>), "T");
            Assert.NotNull(result0);
            var result1 = (List<int>)service.GetService(typeof(IEnumerable<int>), "T");
            Assert.NotNull(result1);
            Assert.NotSame(result0, result1);
        }

        [Fact]
        public void TransientWhenObjectAndGetIEnumerableObject()
        {
            var service = new ServiceDefintions()
                .AddTransient(i => this, "T")
                .AddSingleton(i => new NamedTransientTest())
                .BuildServiceProvider();

            var resultList = (IEnumerable<NamedTransientTest>)service.GetService(typeof(IEnumerable<NamedTransientTest>), "T");
            var result0 = resultList.ToArray();
            Assert.Single(resultList);
            Assert.Same(this, result0[0]);
            var resultList1 = (IEnumerable<NamedTransientTest>)service.GetService(typeof(IEnumerable<NamedTransientTest>), "T");
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
                .AddTransient<IEnumerable<int>, List<int>>("T")
                .AddSingleton<IEnumerable<int>, List<int>>()
                .BuildServiceProvider();

            var resultList = (IEnumerable<int>)service.GetService(typeof(IEnumerable<int>), "T");
            Assert.Empty(resultList);
            var resultList1 = (IEnumerable<int>)service.GetService(typeof(IEnumerable<int>), "T");
            Assert.Empty(resultList1);
            Assert.NotSame(resultList, resultList1);
        }

        [Fact]
        public void TransientWhenObjectAndGetIEnumerableList()
        {
            var service = new ServiceDefintions()
                .AddTransient(i => this, "T")
                .AddSingleton(i => new NamedTransientTest())
                .BuildServiceProvider();

            var resultList = (IEnumerable<IEnumerable<NamedTransientTest>>)service.GetService(typeof(IEnumerable<IEnumerable<NamedTransientTest>>), "T");
            var result00 = resultList.ToArray();
            var result0 = result00[0].ToArray();
            Assert.Single(resultList);
            Assert.Single(result0);
            Assert.Same(this, result0[0]);
            var resultList1 = (IEnumerable<IEnumerable<NamedTransientTest>>)service.GetService(typeof(IEnumerable<IEnumerable<NamedTransientTest>>), "T");
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